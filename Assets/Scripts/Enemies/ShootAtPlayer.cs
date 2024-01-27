using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to make enemies shoot at the player
public class ShootAtPlayer : MonoBehaviour
{
    private StatManager statManager;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private GameObject _bulletSpawn;

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

        statManager = GetComponent<StatManager>();
        if (statManager == null)
            Debug.Log("No Statmanger found for " + this.gameObject.name);
    }

    void FixedUpdate()
    {
        if (Time.time > _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + 1 / statManager.FireRate;
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
                basicBullet.speed = statManager.BulletSpeed;
                basicBullet.damage = statManager.Damage;
            }
        }
    }
}
