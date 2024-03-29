using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to make game objects look at the player
public class LookAtPlayer : MonoBehaviour
{
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
    }

    private void FixedUpdate()
    {
        if (this._target != null)
        {
            Vector2 direction = this._target.position - this.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, 1f);
        }
    }
    
}
