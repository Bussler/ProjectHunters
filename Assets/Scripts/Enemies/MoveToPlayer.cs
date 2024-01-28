using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Class to make enemies move towards the player
public class MoveToPlayer : BasicMovement
{
    private Transform _target;

    [SerializeField]
    private int cellIndex;

    protected override void Start()
    {
        base.Start();
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            this._target = player.transform;
        }
        else
        {
            Debug.Log("No GameObject with 'Player' tag found in the scene. Target not found for " + this.gameObject.name);
        }

        cellIndex = SpacePartitionManager.Instance.AddObject(this.gameObject);
    }

    void FixedUpdate()
    {
        if (this._target != null)
        {
            Vector2 direction = this._target.position - this.transform.position;
            Move(direction.normalized);

            PushNearbyEnemies();

            UpdateSpatialGroup();
        }
    }

    private void PushNearbyEnemies()
    {

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject hitObject = collision.gameObject;

        if (hitObject.CompareTag("Player"))
        {
            StatManager playerStats = hitObject.GetComponent<StatManager>();
            if (playerStats != null && statManager != null)
            {
                int dealt_damage = statManager.Damage;
                playerStats.TakeDamage(dealt_damage);
            }
            ObjectPoolManager.Instance.DespawnObject(this.gameObject);
        }
    }

}
