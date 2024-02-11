using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

// Class to handle item drops, e.g. when an enemy dies
public class DropItem : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _passiveItemPrefabs = new List<GameObject>();

    [SerializeField]
    private float _dropChance = 0.1f;

    public void Drop()
    {
        if (Random.Range(0f, 1f) <= _dropChance)
        {
            GameObject itemPrefab = _passiveItemPrefabs[Random.Range(0, _passiveItemPrefabs.Count)];

            spawnPassiveItem(itemPrefab);
        }
    }

    protected void spawnPassiveItem(GameObject itemPrefab)
    {
        bool addingSucceeded = InventoryManager.Instance.AddPassiveItem(itemPrefab);
        if (!addingSucceeded)
        {
            Debug.Log("Adding passive item to inventory failed!");
        }
    }
}
