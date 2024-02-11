using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage Passive Items, these affect Player stats: health, damage, movement ...
public class PassiveItem : MonoBehaviour
{
    public string itemName;
    public PassiveItemScriptableObject passiveItemData;
    public int level = 1;
    protected StatManager appliedStatManager;
    protected bool isApplied = false;

    protected virtual void ApplyPassiveItem()
    {
        // Apply passive item to player
        // Overwrite this method in child classes
    }

    protected virtual void RemovePassiveItem()
    {
        // Remove passive item from player
        // Overwrite this method in child classes
    }

    // Level up the passive item and apply the new stats
    public void LevelUpPassiveItem()
    {
        if (!isApplied)
        {
            Debug.LogError("Leveling Up Passive Item before it is applied!");
            return;
        }

        RemovePassiveItem();
        level++;
        ApplyPassiveItem();
    }

    void Start()
    {
        if (itemName == "")
        {
            itemName = this.gameObject.name;
        }

        appliedStatManager = GetComponentInParent<StatManager>();
        ApplyPassiveItem();
        isApplied = true;
    }

    void OnDestroy()
    {
        RemovePassiveItem();
    }

}
