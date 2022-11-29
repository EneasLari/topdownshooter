using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spawner : MonoBehaviour
{

    public Wave[] waves;
    public Enemy enemy;

    public Wave currentWave;
    int currentWaveNumber;

    int enemiesRemainingToSpawn;
    int enemiesRemainingAlive;
    float nextSpawnTime;

    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime) { 
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time+currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    void OnEnemyDeath() {
        print("Enemy Died");
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0) {
            NextWave();
        }
    }

    void NextWave() {
        currentWaveNumber++;
        if (currentWaveNumber - 1 < waves.Length) {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }

    }

    //classa to store variables of each wave of enemies
    [System.Serializable]
    public class Wave { 
        public int enemyCount;
        public float timeBetweenSpawns;
    }
}
