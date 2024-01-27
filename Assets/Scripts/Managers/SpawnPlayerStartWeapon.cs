using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerStartWeapon : MonoBehaviour
{
    public List<GameObject> startWeaponPrefabs;

    private GameObject player;
    private GameObject weaponHolster;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpawnPlayerWeapons();
        }
    }

    void SpawnPlayerWeaponsHolster()
    {
        weaponHolster = new GameObject("Weapon Holster");
        weaponHolster.transform.SetParent(player.transform);
    }

    void SpawnPlayerWeapons()
    {
        if(weaponHolster == null)
            SpawnPlayerWeaponsHolster();
       
        for (int i = 0; i < startWeaponPrefabs.Count; i++)
        {
            GameObject weapon = Instantiate(startWeaponPrefabs[i], weaponHolster.transform.position, Quaternion.identity);
            weapon.transform.parent = weaponHolster.transform;

            InventoryManager.Instance.AddWeapon(weapon);
        }
    }
}
