using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to make enemies shoot at the player
public class ShootAtPlayer : MonoBehaviour
{
    public int damage = 10;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private GameObject _bulletSpawn;

    [SerializeField]
    private float _bulletSpeed = 20f;

    [SerializeField]
    private float _fireRate = 1f;

    private Transform _target;
    private float _nextFireTime = 0f;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            this._target = player.transform;
        }
        else
        {
            Debug.Log("No GameObject with 'Player' tag found in the scene. Target not found for " + this.gameObject.name);
        }
    }

    void FixedUpdate()
    {
        if (Time.time > _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + 1 / _fireRate;
        }
    }

    void Shoot()
    {
        if (_target != null)
        {
            // calculate quaternion to look at player
            Vector2 direction = this._target.position - this.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            GameObject bullet = ObjectPoolManager.Instance.SpawnObject(_bulletPrefab, _bulletSpawn.transform.position, rotation, ObjectPoolManager.PoolType.EnemyBullet);

            BasicBullet basicBullet = bullet.GetComponent<BasicBullet>();
            if (basicBullet != null)
            {
                basicBullet.speed = _bulletSpeed;
                basicBullet.damage = damage;
            }
        }
    }
}
