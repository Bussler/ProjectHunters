using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 5;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Move forward
    void FixedUpdate()
    {
        _rb.velocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject hitObject = other.gameObject;
        
        if (hitObject.CompareTag("Bullet") || hitObject.CompareTag("EnemyBullet")) // Ignore other bullets TODO: Do with collision layer
        {
            return;
        }
        else if (hitObject.CompareTag("Player"))
        {
            PlayerStatManager playerStats = hitObject.GetComponent<PlayerStatManager>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
            ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
        }
        else if (hitObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = hitObject.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }
        }

        ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
    }

}
