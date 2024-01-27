using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/PassiveItem")]
public class PassiveItemScriptableObject : ScriptableObject
{
    public float basicValueIncrease; // Value to increase the stat by
    public float levelValueMultiplier = 1; // Value to multiply the bascicValueIncrease by when leveling up

    public float calculateIncrease(int level = 1)
    {
        return basicValueIncrease * (level * levelValueMultiplier);
    }
}
