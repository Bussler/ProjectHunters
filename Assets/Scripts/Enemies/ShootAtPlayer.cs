using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAtPlayer : MonoBehaviour
{
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
            Vector3 direction = (_target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(transform.up); // TODO something is not working with these quaternions

            GameObject bullet = ObjectPoolManager.Instance.SpawnObject(_bulletPrefab, _bulletSpawn.transform.position, lookRotation, ObjectPoolManager.PoolType.Bullet);

            BasicBullet basicBullet = bullet.GetComponent<BasicBullet>();
            if (basicBullet != null)
            {
                basicBullet.speed = _bulletSpeed;

                EnemyStats enemyStats = GetComponent<EnemyStats>();
                if (enemyStats != null)
                    basicBullet.damage = enemyStats.damage;
            }
        }
    }
}
