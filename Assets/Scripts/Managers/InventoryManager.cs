using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

// Class to manage the player's inventory: weapons, passive items ... Implements Singleton pattern
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public int[] weaponLevels = new int[6]; // TODO
    public PlayerWeapon[] weaponItems = new PlayerWeapon[6]; // TODO

    public List<int> passiveItemsLevels = new List<int>();
    public List<PassiveItem> passiveItems = new List<PassiveItem>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        Instance.weaponLevels = new int[6];
        Instance.weaponItems = new PlayerWeapon[6];
    }

    public bool AddWeapon(PlayerWeapon weapon, int slotIndex)
    {
        if(weaponItems.Length < slotIndex)
        {
            weaponItems[slotIndex] = weapon;
            weaponLevels[slotIndex] = 1;
            return true;
        }

        Debug.Log("Weapon-Inventory full!");
        return false;
    }

    // Search for the first empty slot and add the weapon there
    public bool AddWeapon(PlayerWeapon weapon)
    {
        for (int i = 0; i < weaponItems.Length; i++)
        {
            if (weaponItems[i] == null)
            {
                weaponItems[i] = weapon;
                weaponLevels[i] = 1;
                return true;
            }
        }

        Debug.Log("Weapon-Inventory full!");
        return false;
    }

    public bool RemoveWeapon(int slotIndex)
    {
        weaponItems[slotIndex] = null;
        weaponLevels[slotIndex] = 0;

        return true;
    }

    public bool AddPassiveItem(PassiveItem passiveItem)
    {
        passiveItems.Add(passiveItem);
        passiveItemsLevels.Add(1);

        return true;
    }

    public bool RemovePassiveItem(PassiveItem passiveItem)
    {
        int index = passiveItems.IndexOf(passiveItem);
        if (index != -1)
        {
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
