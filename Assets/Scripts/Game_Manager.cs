using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public Button startButton; 
    public SpawnManager spawnManager;
    public bool isGameActive = false;
    void Start()
    {
        startButton = GameObject.Find("startButton").GetComponent<Button>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {

        isGameActive = true;
        spawnManager.spawnEnemyWave(spawnManager.waveNumber);  
        Instantiate(spawnManager.powerUpPrefab, spawnManager.GenerateSpawnPosition(), spawnManager.powerUpPrefab.transform.rotation);
    }


}
