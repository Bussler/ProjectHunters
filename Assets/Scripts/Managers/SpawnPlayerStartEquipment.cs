using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to spawn the player's start equipment: weapons, passive items ...
public class SpawnPlayerStartEquipment : MonoBehaviour
{
    public List<GameObject> startWeaponPrefabs;
    public List<GameObject> startPassiveItemPrefabs;

    private GameObject player;
    private GameObject weaponHolster;
    private GameObject itemBag;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpawnPlayerWeapons();
            SpawnPlayerItems();
        }
    }

    void SpawnPlayerWeaponsHolster()
    {
        weaponHolster = new GameObject("Weapon Holster");
        weaponHolster.transform.SetParent(player.transform);
    }

    void SpawnPlayerWeapons()
    {
        if(weaponHolster == null)
            SpawnPlayerWeaponsHolster();
       
        for (int i = 0; i < startWeaponPrefabs.Count; i++)
        {
            GameObject weapon = Instantiate(startWeaponPrefabs[i], weaponHolster.transform.position, Quaternion.identity);
            weapon.transform.parent = weaponHolster.transform;

            PlayerWeapon playerWeapon = weapon.GetComponent<PlayerWeapon>();
            if (playerWeapon != null)
            {
                bool addingSucceeded = InventoryManager.Instance.AddWeapon(playerWeapon);
                if (!addingSucceeded)
                {
                    Destroy(weapon);
                }
            }
            else
            {
                Debug.Log("Weapon is not a PlayerWeapon!");
                Destroy(weapon);
            }

        }
    }

    void SpawnPlayerItemsBag()
    {
        itemBag = new GameObject("Item Bag");
        itemBag.transform.SetParent(player.transform);
    }

    void SpawnPlayerItems()
    {
        if (itemBag == null)
            SpawnPlayerItemsBag();

        for (int i = 0; i < startPassiveItemPrefabs.Count; i++)
        {
            GameObject item = Instantiate(startPassiveItemPrefabs[i], itemBag.transform.position, Quaternion.identity);
            item.transform.parent = itemBag.transform;

            PassiveItem passiveItem = item.GetComponent<PassiveItem>();
            if (passiveItem != null)
            {
                bool addingSucceeded = InventoryManager.Instance.AddPassiveItem(passiveItem);
                if (!addingSucceeded)
                {
                    Destroy(item);
                }
            }
            else { 
                Debug.Log("Item is not a PassiveItem!");
                Destroy(item);
            }
        }
    }
}
