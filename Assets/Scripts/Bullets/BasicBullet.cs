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
            StatManager playerStats = hitObject.GetComponent<StatManager>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

            PlayerMovement playerMovement = hitObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.forceToApply += _rb.velocity * impactForce;
            }

            ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
        }
        else if (hitObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy");
            StatManager enemyStats = hitObject.GetComponent<StatManager>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
            }
        }

        ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
    }

}
