using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage basic bullet behaviour: move forward, deal damage to player or enemy
public class BasicBullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 5;
    public float impactForce = 4f;
    public int timeToLive = 3;
    public LayerMask possibleHits; // Layer mask to check for collisions

    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize()
    {
        StartCoroutine(DeleteAfterSeconds(timeToLive));
    }

    IEnumerator DeleteAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
    }

    // Move forward
    void FixedUpdate()
    {
        _rb.velocity = transform.up * speed;
        CheckForCollisionRaycast();
    }

    // check for collision with player or enemy with a raycast
    private void CheckForCollisionRaycast()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 0.5f, possibleHits);
        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            HandleCollision(hitObject);
        }
    }

    // Deprecated: check for collision with player or enemy with collider
    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject hitObject = other.gameObject;
        HandleCollision(hitObject);
    }

    void HandleCollision(GameObject hitObject)
    {
        if (hitObject.CompareTag("Bullet") || hitObject.CompareTag("EnemyBullet"))
        {
            ObjectPoolManager.Instance.DespawnObject(hitObject);
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

        ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
    }

}
