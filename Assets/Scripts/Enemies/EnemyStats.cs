using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int damage = 10;

    [SerializeField]
    private int _max_health = 100;
    private int _min_health = 1;

    [SerializeField]
    private int _health;

    public int Health
    {
        get { return _health; }
        set
        {
            if (value < 0)
                _health = 0;
            else
                _health = value;
        }
    }

    private void Start()
    {
        Initilize();
    }

    public void Initilize()
    {
        Health = _max_health;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (isDead())
        {
            Die();
        }
    }

    public bool isDead()
    {
        return Health <= _min_health;
    }

    public void Die()
    {
        ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
        // Destroy(gameObject);
    }
}
