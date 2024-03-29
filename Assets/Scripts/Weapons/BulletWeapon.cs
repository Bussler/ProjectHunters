using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all player bullet based weapons
public class BulletWeapon : PlayerWeapon
{
    protected BulletWeaponStats bulletWeaponStats;

    protected float _nextFireTime = 0f;

    protected override void Start()
    {
        base.Start();
        bulletWeaponStats = weaponStats as BulletWeaponStats;
    }

    // Shoot at a fixed rate: 1 / FireRate
    protected void FixedUpdate()
    {
        if (Time.time > _nextFireTime)
        {
            _nextFireTime = Time.time + 1 / statManager.FireRate;
            Shoot();
        }
    }
    protected virtual void Shoot()
    {
        // Overwrite this method in child classes for concrete shooting behaviour
    }

    protected Vector2 getShootingDirection(GameObject enemy)
    {
        Vector2 direction = enemy.transform.position - this.transform.position;
        return direction.normalized;
    }

    protected void ShootAt(Vector2 direction)
    {
        // calculate quaternion to look at direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        GameObject bullet = ObjectPoolManager.Instance.SpawnObject(bulletWeaponStats.WeaponPrefab, this.transform.position, rotation, ObjectPoolManager.PoolType.Bullet);

        BasicBullet basicBullet = bullet.GetComponent<BasicBullet>();
        if (basicBullet != null)
        {
            basicBullet.speed = statManager.BulletSpeed;
            basicBullet.damage = getCurrentDamage();
            basicBullet.Initialize();
        }
    }

}
