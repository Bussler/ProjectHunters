using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to make enemies dash towards the player
public class DashEnemy : MonoBehaviour
{
    private MovementDash dash_script;
    private MoveToPlayer enemyMovement;

    void Start()
    {
        dash_script = GetComponent<MovementDash>();
        enemyMovement = GetComponent<MoveToPlayer>();

        if (dash_script != null && enemyMovement != null)
        {
            dash_script.OnDashingChanged += enemyMovement.setCanMove;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (dash_script != null && enemyMovement != null)
            {
                StartCoroutine(dash_script.Dash(collision.transform.position - transform.position));
            }
        }
    }

}
