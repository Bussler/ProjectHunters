using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to make enemies move towards the player
public class MoveToPlayer : MonoBehaviour
{
    public int damage = 10;

    [SerializeField]
    private int _speed = 5;

    private Transform _target;
    private Rigidbody2D _rb;

    private bool canMove = true;

    public Vector2 forceToApply = Vector2.zero; // used for knockback if a projectile hits the player
    [SerializeField]
    private float forceDamping = 1.2f;

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
        if (_target != null && canMove)
        {
            //float step = Time.deltaTime * _speed;
            //transform.position = Vector2.MoveTowards(transform.position, _target.position, step);

            Vector2 direction = this._target.position - this.transform.position;
            Vector2 moveForce = direction.normalized * _speed;
            moveForce += forceToApply;
            forceToApply /= forceDamping;
            if (Mathf.Abs(forceToApply.x) <= 0.01f && Mathf.Abs(forceToApply.y) <= 0.01f)
            {
                forceToApply = Vector2.zero;
            }

            _rb.velocity = moveForce; //direction.normalized * _speed;
        }
    }
    public void setCanMove(bool value)
    {
        this.canMove = !canMove;
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
