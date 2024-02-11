using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to spawn the player's start equipment: weapons, passive items ...
public class SpawnPlayerStartEquipment : DropItem
{
    public List<GameObject> startWeaponPrefabs;
    public List<GameObject> startPassiveItemPrefabs;

    private GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpawnPlayerWeapons();
            SpawnPlayerItems();
        }
        else
        {
            Debug.LogError("Player not found for Spawn Inventory!");
        }
    }

    void SpawnPlayerWeapons()
    {
        for (int i = 0; i < startWeaponPrefabs.Count; i++)
        {
            GameObject currentPrefab = startWeaponPrefabs[i];

            bool addingSucceeded = InventoryManager.Instance.AddWeapon(currentPrefab);
            if (!addingSucceeded)
            {
                Debug.Log("Adding weapon to inventory failed!");
            }

        }
    }

    void SpawnPlayerItems()
    {
        for (int i = 0; i < startPassiveItemPrefabs.Count; i++)
        {
            spawnPassiveItem(startPassiveItemPrefabs[i]);
        }
    }
}
