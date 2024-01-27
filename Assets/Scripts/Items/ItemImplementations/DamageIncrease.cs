using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIncrease : PassiveItem
{
    protected override void ApplyPassiveItem()
    {
        appliedStatManager.Damage += (int)passiveItemData.valueIncrease;
    }

    protected override void RemovePassiveItem()
    {
        appliedStatManager.Damage -= (int)passiveItemData.valueIncrease;
    }
}
