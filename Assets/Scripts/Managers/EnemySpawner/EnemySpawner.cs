using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for spawning enemies in waves
public class EnemySpawner : MonoBehaviour
{
    public List<Wave> waves; // List of waves to spawn

    [SerializeField]
    private Vector2 spawnInterval; // Position where enemies should spawn ([-x, x], [-y, y])

    private int currentWave; // Index of current wave

    void Start()
    {
        foreach (Wave wave in waves)
        {
            if (wave.waveQuota == 0)
                wave.CalculateWaveQuota();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO : Spawn enemies from current wave

        // TODO : Check if current wave is finished

        // TODO : Timer to spawn next wave
    }

    // Spawn enemies from current wave
    private void SpawnWave()
    {
        Wave wave = waves[currentWave];
        if (wave.spawnCount < wave.waveQuota)
        {
            foreach (EnemyGroup enemyGroup in wave.enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    // Spawn enemy
                    Vector2 spawnPosition = new Vector2(Random.Range(-spawnInterval.x, spawnInterval.x), Random.Range(-spawnInterval.y, spawnInterval.y));
                    GameObject gameObject = ObjectPoolManager.Instance.SpawnObject(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity, ObjectPoolManager.PoolType.Enemy);

                    enemyGroup.spawnCount++;
                    wave.spawnCount++;
                }
            }
        }
    }
}
