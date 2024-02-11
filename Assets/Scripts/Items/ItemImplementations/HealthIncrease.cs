using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIncrease : PassiveItem
{
    protected override void ApplyPassiveItem()
    {
        appliedStatManager.MaxHealth += (int)passiveItemData.calculateIncrease(level);
    }

    protected override void RemovePassiveItem()
    {
        appliedStatManager.MaxHealth -= (int)passiveItemData.calculateIncrease(level);
    }
}
