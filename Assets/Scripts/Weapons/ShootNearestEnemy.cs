using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Find the nearest enemies and shoot them
public class ShootNearestEnemy : BulletWeapon
{
    private Dictionary<int, GameObject> _enemiesInRange = new Dictionary<int, GameObject>();

    void Awake()
    {
        _enemiesInRange = new Dictionary<int, GameObject>();
    }

    // shoot at nearest enemies
    protected override void Shoot() {
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
        for (int i = 0; i < statManager.AmountBullets; i++)
        {
            if (i < enemies.Count)
            {
                ShootAt(getShootingDirection(enemies[i]));
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyBullet"))
        {
            int id = other.gameObject.GetInstanceID();
            if (!_enemiesInRange.ContainsKey(id))
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
