using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayer : MonoBehaviour
{
    [SerializeField]
    private int _speed = 5;

    private Transform _target;
    private Rigidbody2D _rb;

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

        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (_target != null)
        {
            //float step = Time.deltaTime * _speed;
            //transform.position = Vector2.MoveTowards(transform.position, _target.position, step);

            Vector2 direction = this._target.position - this.transform.position;
            _rb.velocity = direction.normalized * _speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObject = collision.gameObject;

        if (hitObject.CompareTag("Player"))
        {
            PlayerStatManager playerStats = hitObject.GetComponent<PlayerStatManager>();
            if (playerStats != null)
            {
                EnemyStats enemyStats = GetComponent<EnemyStats>();
                if (enemyStats != null)
                    playerStats.TakeDamage(enemyStats.damage);
            }
            ObjectPoolManager.Instance.DespawnObject(this.gameObject);
        }
    }

}
