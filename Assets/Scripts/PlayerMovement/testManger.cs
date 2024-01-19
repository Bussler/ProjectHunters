using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testManger : MonoBehaviour
{

    public GameObject something;

    // Start is called before the first frame update
    void Start()
    {
        GameObject guy = ObjectPoolManager.Instance.SpawnObject(something, new Vector3(5, 0, 0), Quaternion.identity, ObjectPoolManager.PoolType.Enemy);

        StartCoroutine(ExecuteAfterTime(2, guy));
    }

    IEnumerator ExecuteAfterTime(float time, GameObject guy)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        Debug.Log("Executed after delay: " + time);
        //ObjectPoolManager.Instance.DespawnObject(guy);
    }

}
