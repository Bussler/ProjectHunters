using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : PassiveItem
{
    protected override void ApplyPassiveItem()
    {
        appliedStatManager.Health += (int)passiveItemData.calculateIncrease(1);
    }

    protected override void RemovePassiveItem()
    {
        appliedStatManager.Health -= (int)passiveItemData.calculateIncrease(1);
    }
}
