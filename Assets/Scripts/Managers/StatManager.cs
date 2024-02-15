using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage Player stats: health, death
public class StatManager : MonoBehaviour
{
    private bool isTargetable = true;

    [Header("Damage Attributes")]
    [SerializeField]
    private int _damage = 10;
    [SerializeField]
    private float _bulletSpeed = 20f;
    [SerializeField]
    private int _amountBullets = 20;
    [SerializeField]
    private float _fireRate = 1f;

    [Header("Movement Attributes")]
    [SerializeField]
    private float _moveSpeed = 7f;
    [SerializeField]
    private float _dashingPower = 20f;
    [SerializeField]
    private float _dashingTime = 0.2f;
    [SerializeField]
    private float _dashingCooldown = 1f;

    [Header("Health Attributes")]
    [SerializeField]
    private int _max_health = 100;
    private int _min_health = 0;

    [SerializeField]
    private int _health;

    // encapsulated private fiels
    public int Health
    {
        get { return _health; }
        set
        {
            if (value <= _min_health)
                _health = _min_health;
            else if (value >= _max_health)
                _health = _max_health;
            else
                _health = value;
        }
    }

    public bool IsTargetable { get => isTargetable; set => isTargetable = value; }
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public int MaxHealth { get => _max_health; set => _max_health = value; }
    public int Damage { get => _damage; set => _damage = value; }
    public float BulletSpeed { get => _bulletSpeed; set => _bulletSpeed = value; }
    public int AmountBullets { get => _amountBullets; set => _amountBullets = value; }
    public float FireRate { get => _fireRate; set => _fireRate = value; }
    public float DashingTime { get => _dashingTime; set => _dashingTime = value; }
    public float DashingCooldown { get => _dashingCooldown; set => _dashingCooldown = value; }
    public float DashingPower { get => _dashingPower; set => _dashingPower = value; }

    private void Awake()
    {
        Initilize();
    }

    public void Initilize()
    {
        Health = MaxHealth;
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
            LeaderBoardManager.instance.UploadScore();
        }
        else // Enemy
        {
            ObjectPoolManager.Instance.DespawnObject(this.gameObject); // Instead of destroy, deactivation in pool
            EnemySpawner.Instance.OnEnemyDied();

            LeaderBoardManager.instance.AddScore(100); // TODO: Make dynamic later
            LeaderBoardManager.instance.UploadScore();

            DropItem dropItem = GetComponent<DropItem>();
            if (dropItem != null)
            {
                dropItem.Drop();
            }
        }

    }
}
