using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage basic bullet behaviour: move forward, deal damage to player or enemy
public class BasicBullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 5;
    public float impactForce = 4f;
    private Rigidbody2D _rb;

    [SerializeField]
    private int cellIndex;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        cellIndex = SpacePartitionManager.Instance.AddObject(this.gameObject);
    }

    // Move forward
    void FixedUpdate()
    {
        _rb.velocity = transform.up * speed;

        UpdateSpatialGroup();
        CustomCheckCollision();
    }

    private void CustomCheckCollision()
    {
        List<int> nearbyCells = SpacePartitionManager.GetNearbyCells(cellIndex);
        List<GameObject> nearbyEnemies = SpacePartitionManager.GetAllObjectsInGridGroups(nearbyCells);

        foreach (GameObject enemy in nearbyEnemies)
        {
            if (enemy != null && enemy != this.gameObject)
            {
                float distance = Vector2.Distance(enemy.transform.position, this.transform.position);
                if (Mathf.Abs(distance) < 1f)
                {
                    HandleCollision(enemy);
                }
            }
        }
    }

    private void UpdateSpatialGroup()
    {
        int newCellIndex = SpacePartitionManager.Instance.GetCellIndex(this.transform.position);
        if (newCellIndex != cellIndex)
        {
            SpacePartitionManager.Instance.RemoveObject(this.gameObject, cellIndex);

            cellIndex = newCellIndex;

            SpacePartitionManager.Instance.AddObject(this.gameObject, cellIndex);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject hitObject = other.gameObject;
        HandleCollision(hitObject);
    }

    private void HandleCollision(GameObject hitObject)
    {
        if (hitObject.CompareTag("Bullet") || hitObject.CompareTag("EnemyBullet"))
        {
            // if bullet hits another bullet, destroy both
            //ObjectPoolManager.Instance.DespawnObject(hitObject);
        }
        else if (hitObject.CompareTag("Player") || hitObject.CompareTag("Enemy"))
        {
            StatManager stats = hitObject.GetComponent<StatManager>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
            }

            BasicMovement movement = hitObject.GetComponent<BasicMovement>();
            if (movement != null)
            {
                movement.forceToApply += _rb.velocity.normalized * impactForce;
            }
        }

        // TODO activate again, check in beginning if we collide with our own bullets and then dont collide
        //ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
    }

}
