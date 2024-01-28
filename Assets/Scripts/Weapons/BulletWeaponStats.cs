using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicWeaponStats", menuName = "ScriptableObjects/Weapon/BulletWeapon")]
public class BulletWeaponStats : BasicWeaponStats
{
    // TODO placeholder stat for now, not currently used!
    [SerializeField]
    private int pierce = 1;

    [SerializeField]
    private float bulletSpeed = 1;

    [SerializeField]
    private float fireRate = 1;

    public int Pierce { get => pierce; protected set => pierce = value; }
    public float BulletSpeed { get => bulletSpeed; protected set => bulletSpeed = value; }
    public float FireRate { get => fireRate; protected set => fireRate = value; }
}
