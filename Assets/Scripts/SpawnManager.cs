using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerUpPrefab;
    public Game_Manager manager;
    public Python_Client python_Client;

    private float spawnRange = 9.0f;
    public int enemyCount;
    public int waveNumber = 1;


    
    void Update()
    {
        if (manager != null && manager.isGameActive)
        {
            enemyCount = FindObjectsOfType<Enemy>().Length;

            if (enemyCount == 0)
            {
                // Send data to server each wave
                float performance = Random.Range(0.3f, 1.0f); // Random generation of performence
                python_Client.AskPythonAsync(waveNumber, performance);

                waveNumber++;
                spawnEnemyWave(waveNumber);
                Instantiate(powerUpPrefab, GenerateSpawnPosition(), powerUpPrefab.transform.rotation);
            }
        }
    }

    //Instantiation of enemies
    public void spawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }
    // Randomization of spawn locations
    public Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}
