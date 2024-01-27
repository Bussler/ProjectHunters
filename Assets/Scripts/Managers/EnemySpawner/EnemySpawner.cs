using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is responsible for spawning enemies in waves. Set up as singleton pattern
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    public List<Wave> waves; // List of waves to spawn

    [Header("Spawner Attributes")]
    [SerializeField]
    private float waveInterval; // Time between waves

    [SerializeField]
    private int maxEnemiesAllowed; // Maximum number of enemies allowed to be alive at once

    [SerializeField]
    private Vector2 spawnPositionInterval; // Position where enemies should spawn ([-x, x], [-y, y])

    private int currentWave; // Index of current wave
    private float spawnTimer; // Timer to check spawn of next enemy in current wave
    private int enemiesAlive; // Number of enemies currently alive
    private bool waitingForNextWave = false; // Flag to check if waiting for next wave


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        foreach (Wave wave in waves)
        {
            if (wave.waveQuota == 0)
                wave.CalculateWaveQuota();
        }
    }

    // TODO here is a bug with the waves: Some wave not spawning
    // Update is called once per frame
    void FixedUpdate()
    {
        //Check if current wave is finished: if all enemies have been spawned, move to next wave
        if (currentWave < waves.Count - 1 && waves[currentWave].spawnCount >= waves[currentWave].waveQuota && !waitingForNextWave)
        {
            waitingForNextWave = true;
            StartCoroutine(BeginNextWave());
        }

        //Spawn enemies from current wave
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= waves[currentWave].spawnInterval)
        {
            spawnTimer = 0;
            SpawnWave();
        }

    }

    // Coroutine to start next wave after waveInterval
    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);

        if (currentWave < waves.Count - 1)
        {
            currentWave++;
            spawnTimer = 0;
        }
        waitingForNextWave = false;
    }

    // Spawn enemies from current wave: Go through each enemy group and spawn enemies
    private void SpawnWave()
    {
        Wave wave = waves[currentWave];
        if (wave.spawnCount < wave.waveQuota)
        {
            foreach (EnemyGroup enemyGroup in wave.enemyGroups)
            {
                for (int i = 0; i < enemyGroup.spawnIntensity; i++)
                {
                    if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                    {
                        if (enemiesAlive >= maxEnemiesAllowed)
                            return;

                        // Spawn enemy
                        Vector2 spawnPosition = new Vector2(Random.Range(-spawnPositionInterval.x, spawnPositionInterval.x), Random.Range(-spawnPositionInterval.y, spawnPositionInterval.y));
                        GameObject gameObject = ObjectPoolManager.Instance.SpawnObject(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity, ObjectPoolManager.PoolType.Enemy);

                        StatManager enemyStats = gameObject.GetComponent<StatManager>();
                        if (enemyStats != null)
                            enemyStats.Initilize();

                        enemyGroup.spawnCount++;
                        wave.spawnCount++;
                        enemiesAlive++;
                    }
                }
            }
        }
        // TODO : Check if current wave is finished: All enemies have been killed
        // M: For that we would have to keep some sort of map for each wave;
        // lets keep a max amount enmies for now and spawn next wave when current wave is finished spawning
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
    }
}
