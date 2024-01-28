using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicWeaponStats", menuName = "ScriptableObjects/Weapon/BasicWeapon")]
public class BasicWeaponStats : ScriptableObject
{
    [SerializeField]
    protected GameObject weaponPrefab;

    [SerializeField]
    protected int basicWeaponDamage = 5;

    public GameObject WeaponPrefab { get => weaponPrefab; protected set => weaponPrefab = value; }
    public int BasicWeaponDamage { get => basicWeaponDamage; protected set => basicWeaponDamage = value; }
}
