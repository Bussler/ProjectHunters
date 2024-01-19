using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Linq;

// Find the nearest enemies and shoot them
public class ShootNearestEnemy : MonoBehaviour
{
    public int damage = 10;
    private Dictionary<int, GameObject> _enemiesInRange = new Dictionary<int, GameObject>();

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
        _enemiesInRange = new Dictionary<int, GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Shoot();
    }

    private void Shoot() {
        if (Time.time > _nextFireTime)
        {
            _nextFireTime = Time.time + 1 / _fireRate;

            // remove inactive enemies from dictionary
            _enemiesInRange = _enemiesInRange
                .Where(pair => pair.Value.activeInHierarchy)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            List<GameObject> enemies = new List<GameObject>(_enemiesInRange.Values);

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
                    ShootAt(enemies[i]);
                }
            }
        }
    }

    private void ShootAt(GameObject enemy)
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet"))
        {
            int id = other.gameObject.GetInstanceID();
            _enemiesInRange.Add(id, other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet"))
        {
            int id = other.gameObject.GetInstanceID();
            _enemiesInRange.Remove(id);
        }
    }
}
