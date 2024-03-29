using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for all player weapons
public class PlayerWeapon : MonoBehaviour
{
    protected StatManager statManager;

    [SerializeField]
    protected BasicWeaponStats weaponStats;

    public int currentLevel = 1;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        statManager = GetComponentInParent<StatManager>();
        if (statManager == null)
            Debug.Log("No Statmanger found for " + this.gameObject.name);
    }

    public void LevelUp()
    {
        currentLevel++;
    }

    // Damage of the weapon depends on the current level, the weapon damage and the player's damage
    protected int getCurrentDamage()
    {
        return statManager.Damage + (weaponStats.BasicWeaponDamage * currentLevel);
    }

}
