using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                GameObject itemBag = GameObject.Find("Item Bag");
                if (itemBag == null)
                {
                    itemBag = new GameObject("Item Bag");
                    itemBag.transform.SetParent(player.transform);
                }

                spawnPassiveItem(itemPrefab, itemBag.transform);
            }
        }
    }

    protected void spawnPassiveItem(GameObject itemPrefab, Transform itemBag)
    {
        GameObject item = Instantiate(itemPrefab, itemBag.position, Quaternion.identity);
        item.transform.parent = itemBag;

        PassiveItem passiveItem = item.GetComponent<PassiveItem>();
        if (passiveItem != null)
        {
            bool addingSucceeded = InventoryManager.Instance.AddPassiveItem(passiveItem);
            if (!addingSucceeded)
            {
                Destroy(item);
            }
        }
        else
        {
            Debug.Log("Item is not a PassiveItem!");
            Destroy(item);
        }
    }
}
