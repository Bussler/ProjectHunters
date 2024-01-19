using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Find the nearest enemies and shoot them
public class ShootNearestEnemy : MonoBehaviour
{
    public int damage = 10;
    private GameObject _enemyHolder;

    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private float _bulletSpeed = 20f;

    [SerializeField]
    private float _amountBullets = 20f;

    [SerializeField]
    private float _fireRate = 1f;

    private float _nextFireTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _enemyHolder = ObjectPoolManager.Instance._enemyPoolHolder;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Shoot();
    }

    private void Shoot() {
        if (Time.time > _nextFireTime)
        {
            Debug.Log("Shoot");
            _nextFireTime = Time.time + 1 / _fireRate;

            // find nearest enemies
            List<GameObject> enemies = new List<GameObject>();
            foreach (Transform child in _enemyHolder.transform)
            {
                if (child.gameObject.activeSelf)
                    enemies.Add(child.gameObject);
            }

            // sort by distance
            enemies.Sort(delegate (GameObject a, GameObject b)
            {
                return Vector2.Distance(this.transform.position, a.transform.position)
                .CompareTo(Vector2.Distance(this.transform.position, b.transform.position)
                );
            });

            // shoot at nearest enemies
            for (int i = 0; i < _amountBullets; i++)
            {
                if (i < enemies.Count)
                {
                    GameObject enemy = enemies[i];
                    if (enemy != null)
                    {
                        // calculate quaternion to look at enemy
                        Vector2 direction = enemy.transform.position - this.transform.position;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

                        GameObject bullet = ObjectPoolManager.Instance.SpawnObject(_bulletPrefab, this.transform.position, rotation, ObjectPoolManager.PoolType.Bullet);

                        BasicBullet basicBullet = bullet.GetComponent<BasicBullet>();
                        if (basicBullet != null)
                        {
                            basicBullet.speed = _bulletSpeed;
                            basicBullet.damage = damage;
                        }
                    }
                }
            }
        }
    }
}
