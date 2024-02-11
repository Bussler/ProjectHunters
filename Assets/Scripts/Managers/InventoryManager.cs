using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

// Class to manage the player's inventory: weapons, passive items ... Implements Singleton pattern
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int[] weaponLevels = new int[3];
    public PlayerWeapon[] weaponItems = new PlayerWeapon[3];

    public List<int> passiveItemsLevels = new List<int>();
    public List<PassiveItem> passiveItems = new List<PassiveItem>();

    private GameObject player;
    private GameObject weaponHolster;
    private GameObject itemBag;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        Instance.weaponLevels = new int[3];
        Instance.weaponItems = new PlayerWeapon[3];

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpawnPlayerWeaponsHolster();
            SpawnPlayerItemsBag();
        }
        else
        {
            Debug.LogError("Player not found for Inventory Manager!");
        }
    }

    void SpawnPlayerWeaponsHolster()
    {
        weaponHolster = new GameObject("Weapon Holster");
        weaponHolster.transform.SetParent(player.transform);
    }

    void SpawnPlayerItemsBag()
    {
        itemBag = new GameObject("Item Bag");
        itemBag.transform.SetParent(player.transform);
    }

    private GameObject SpawnPlayerWeapon(GameObject weapon)
    {
        if (weaponHolster == null)
            SpawnPlayerWeaponsHolster();

        GameObject new_weapon = Instantiate(weapon, weaponHolster.transform.position, Quaternion.identity);
        new_weapon.transform.parent = weaponHolster.transform;
        return new_weapon;
    }

    private void DespawnPlayerWeapon(GameObject weapon)
    {
        Destroy(weapon);
    }

    private GameObject SpawnPassiveItem(GameObject item)
    {
        if (itemBag == null)
            SpawnPlayerItemsBag();

        GameObject new_item = Instantiate(item, itemBag.transform.position, Quaternion.identity);
        new_item.transform.parent = itemBag.transform;
        return new_item;
    }

    private void DespawnPassiveItem(GameObject item)
    {
        Destroy(item);
    }

    // Add weapon to the inventory at a specific slot
    // Spawn new weapon game object, if not yet present. Otherwise, level up the weapon or overwrite it
    public bool AddWeapon(GameObject weapon, int slotIndex)
    {
        PlayerWeapon playerWeapon = weapon.GetComponent<PlayerWeapon>();
        if (playerWeapon != null)
        {
            if (weaponItems.Length < slotIndex)
            {
                // spawn new weapon
                if(weaponItems[slotIndex] == null)
                {
                    GameObject newWeapon = SpawnPlayerWeapon(weapon);
                    playerWeapon = newWeapon.GetComponent<PlayerWeapon>();
                    weaponItems[slotIndex] = playerWeapon;
                    weaponLevels[slotIndex] = 1;
                    return true;
                }
                // level up weapon
                else if (weaponItems[slotIndex].gameObject.name == weapon.name)
                {
                    LevelUpWeapon(slotIndex);
                    return true;
                }

            }

            Debug.Log("Weapon-Inventory full!");
            return false;
        }
        else
        {
            Debug.Log("Weapon is not a PlayerWeapon!");
            return false;
        }
    }

    // Search for the first empty slot and add the weapon there
    public bool AddWeapon(GameObject weapon)
    {
        PlayerWeapon playerWeapon = weapon.GetComponent<PlayerWeapon>();
        if (playerWeapon != null)
        {
            // Search for the first empty slot and add the weapon there
            for (int i = 0; i < weaponItems.Length; i++)
            {
                // spawn new weapon
                if (weaponItems[i] == null)
                {
                    GameObject newWeapon = SpawnPlayerWeapon(weapon);
                    playerWeapon = newWeapon.GetComponent<PlayerWeapon>();
                    weaponItems[i] = playerWeapon;
                    weaponLevels[i] = 1;
                    return true;
                }
                // level up weapon
                else if (weaponItems[i].gameObject.name == weapon.name)
                {
                    LevelUpWeapon(i);
                    return true;
                }
            }

            Debug.LogError("Weapon-Inventory full!");
            return false;
        }
        else
        {
            Debug.LogError("Weapon is not a PlayerWeapon!");
            return false;
        }
    }

    public bool RemoveWeapon(int slotIndex)
    {
        DespawnPlayerWeapon(weaponItems[slotIndex].gameObject);
        weaponItems[slotIndex] = null;
        weaponLevels[slotIndex] = 0;

        return true;
    }

    public bool AddPassiveItem(GameObject item)
    {
        PassiveItem passiveItem = item.GetComponent<PassiveItem>();
        if (passiveItem != null)
        {
            // Search for passive item in inventory and level up; Workaround for unity naming: Add (Clone) to the name
            int index = passiveItems.FindIndex(currentItem => currentItem.itemName == item.name+"(Clone)");
            if (index != -1)
            {
                LevelUpPassiveItem(index);
                return true;
            }
            else
            {
                // spawn new item
                GameObject newItem = SpawnPassiveItem(item);
                passiveItem = newItem.GetComponent<PassiveItem>();
                passiveItems.Add(passiveItem);
                passiveItemsLevels.Add(1);
            }
        }
        else
        {
            Debug.Log("Item is not a PassiveItem!");
            return false;
        }

        return true;
    }

    public bool RemovePassiveItem(PassiveItem passiveItem)
    {
        int index = passiveItems.IndexOf(passiveItem);
        if (index != -1)
        {
            // Despawn passive item
            DespawnPassiveItem(passiveItem.gameObject);

            // Remove passive item from list
            passiveItems.RemoveAt(index);

            // Reset level
            passiveItemsLevels.RemoveAt(index);

            return true;
        }

        Debug.Log("Passive item not found in inventory!");
        return false;
    }

    public void LevelUpWeapon(int slotIndex)
    {
        if(weaponLevels.Length < slotIndex)
        {
            weaponLevels[slotIndex]++;
            weaponItems[slotIndex].LevelUp();
        }
    }

    public void LevelUpPassiveItem(int slotIndex)
    {
        if (passiveItems.Count > slotIndex)
        {
            passiveItemsLevels[slotIndex]++;
            passiveItems[slotIndex].LevelUpPassiveItem();
        }
    }
    
}
