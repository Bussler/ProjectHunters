using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage Player stats: health, death
public class StatManager : MonoBehaviour
{
    private bool isTargetable = true;

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

    public bool IsTargetable { get => isTargetable; set => isTargetable = value; }

    private void Awake()
    {
        Initilize();
    }

    public void Initilize()
    {
        Health = _max_health;
    }

    public void TakeDamage(int damage)
    {
        if (!IsTargetable)
            return;

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
        if (gameObject.tag == "Player")
        {
            Debug.Log("Player died");
            // TODO: Game over
        }
        else
        {
            ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
        }

    }
}
