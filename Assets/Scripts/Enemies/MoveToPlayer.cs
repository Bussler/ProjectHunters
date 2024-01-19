using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Class to make enemies move towards the player
public class MoveToPlayer : BasicMovement
{
    public int damage = 10;
    private Transform _target;

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
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (this._target != null)
        {
            Vector2 direction = this._target.position - this.transform.position;
            Move(direction.normalized);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObject = collision.gameObject;

        if (hitObject.CompareTag("Player"))
        {
            StatManager playerStats = hitObject.GetComponent<StatManager>();
            if (playerStats != null)
            {
                    playerStats.TakeDamage(damage);
            }
            ObjectPoolManager.Instance.DespawnObject(this.gameObject);
        }
    }

}
