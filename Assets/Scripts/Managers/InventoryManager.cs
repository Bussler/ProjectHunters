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
    public GameObject[] weaponItems = new GameObject[6]; // TODO

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
        Instance.weaponItems = new GameObject[6];
    }

    public bool AddWeapon(GameObject weapon, int slotIndex)
    {
        if(weaponItems.Length < slotIndex)
        {
            weaponItems[slotIndex] = weapon;
            return true;
        }
        return false;
    }

    public bool AddWeapon(GameObject weapon)
    {
        for (int i = 0; i < weaponItems.Length; i++)
        {
            if (weaponItems[i] == null)
            {
                weaponItems[i] = weapon;
                return true;
            }
        }
        return false;
    }

    public void RemoveWeapon(int slotIndex)
    {
        weaponItems[slotIndex] = null;
    }

    public void AddPassiveItem(PassiveItem passiveItem)
    {
        passiveItems.Add(passiveItem);
    }

    public void RemovePassiveItem(PassiveItem passiveItem)
    {
        passiveItems.Remove(passiveItem);
    }

    public void LevelUpWeapon(int slotIndex)
    {
        weaponLevels[slotIndex]++;
        // TODO: Update weapon stats
    }

    public void LevelUpPassiveItem(int slotIndex)
    {
        passiveItemsLevels[slotIndex]++;
    }
    
}
