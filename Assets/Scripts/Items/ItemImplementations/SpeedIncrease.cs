using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedIncrease : PassiveItem
{
    protected override void ApplyPassiveItem()
    {
        appliedStatManager.MoveSpeed += passiveItemData.calculateIncrease(level);
    }

    protected override void RemovePassiveItem()
    {
        appliedStatManager.MoveSpeed -= passiveItemData.calculateIncrease(level);
    }
}
