using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to manage Passive Items, these affect Player stats: health, damage, movement ...
public class PassiveItem : MonoBehaviour
{
    public PassiveItemScriptableObject passiveItemData;
    protected StatManager appliedStatManager;

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

    void Start()
    {
        appliedStatManager = GetComponentInParent<StatManager>();
        ApplyPassiveItem();
    }

    void OnDestroy()
    {
        RemovePassiveItem();
    }

}
