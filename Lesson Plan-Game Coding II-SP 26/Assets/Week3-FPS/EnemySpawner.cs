using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawner : MonoBehaviour
{
    [FormerlySerializedAs("enemyPrefab")] public GameObject[] enemyPrefabs;
    //predefined spawn locations
    public Transform[] spawnPoints;
    
    //current wave # starts at 1
    public int waveNumber = 1;
    public float timeBetweenWaves = 10f;
    //number of enemies perwave
    public int enemiesPerWave = 3;
    //track number of alive enemies in the scene
    private int enemiesAlive = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //spawn enemies in start is optional, if we want to spawn enemies asap at start of game
        //or wait timebetweenwaves
        SpawnEnemies();
        //start spawning future waves w delay
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        //while runs with a condition but while true always evaluates to true which means it creates infinite loop
        while (true) //inifite loop to continusouly spawn waves
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            SpawnEnemies(); //spawn new enemies after waiting
        }
    }
    private void SpawnEnemies()
    {
        //$ is called string interpolation makes it easier to insert vars into string
        //debug.log(spawning waves: " + waveNumber;
        Debug.Log($"Spawning wave {waveNumber}"); //directly inserts var into string, cleaener
        for(int i = 0; i < enemiesPerWave; i++)
        {
            //pick random spawn point from list
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            //pick random enemy type from the list
            GameObject enemyPrefab = this.enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            //create an enemy at chosen spawn location
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            //increase the count of alive enemies
            enemiesAlive++;

        }
        //increase wave # for next wave
        waveNumber++;
        //difficulty increase enemy count every wave
        enemiesPerWave += 2; 
    }

    public void EnemyDied()
    {
        enemiesAlive--;
        //if all enemies are dead we can trigger a new event
        if (enemiesAlive <= 0)
        {
            Debug.Log("Wave Cleared");
            //do whatever else
        }
    }
}
