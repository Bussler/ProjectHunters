using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Wave Template", menuName = "ScriptableObjects/Wave")]
public class Wave : ScriptableObject
{
    public new string name;
    public List<EnemyGroup> enemyGroups; // List of enemies in this wave
    public int waveQuota; // total number of enemies in this wave. Is calculated automatically, if set to 0
    public float spawnInterval; // How often enemies should spawn

    [System.NonSerialized]
    public float spawnCount; // How many enemies have been spawned

    public void CalculateWaveQuota() // Calculates waveQuota based on enemyGroups
    {
        waveQuota = 0;
        foreach (EnemyGroup enemyGroup in enemyGroups)
        {
            waveQuota += enemyGroup.enemyCount;
        }
    }
}

[System.Serializable]
public class EnemyGroup
{
    public int enemyCount; // How many enemies in this group should be spawned
    public GameObject enemyPrefab; // Enemy prefab to spawn

    [System.NonSerialized]
    public float spawnCount; // How many enemies in this group have been spawned
}
