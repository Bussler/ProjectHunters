using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemy : MonoBehaviour
{
    private MovementDash dash_script;

    void Start()
    {
        dash_script = GetComponent<MovementDash>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(dash_script.Dash(collision.transform.position - transform.position));
        }
    }

}
