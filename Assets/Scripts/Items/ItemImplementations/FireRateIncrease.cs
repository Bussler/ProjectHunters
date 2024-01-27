using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateIncrease : PassiveItem
{
    protected override void ApplyPassiveItem()
    {
        appliedStatManager.FireRate += passiveItemData.calculateIncrease(level);
    }

    protected override void RemovePassiveItem()
    {
        appliedStatManager.FireRate -= passiveItemData.calculateIncrease(level);
    }
}
