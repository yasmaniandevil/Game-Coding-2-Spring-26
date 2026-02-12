using System.Collections;
using UnityEngine;



public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    //current wave # will start at 1
    public int waveNumber = 1;
    public float timeBetweenWaves = 20f;
    //numbers of enemies per wave
    public int enemiesPerWave = 2;
    //track the amount of alive enemies in a scene
    private int enemiesAlive = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //spawn enemies right at start or wait between waves
        //you can turn this off
        SpawnEnemies();
        StartCoroutine(SpawnWave());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemies()
    {
        for(int i = 0; i < enemiesPerWave; i++)
        {
            //pick a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0,spawnPoints.Length)];
            //pick a random enemy type from the list
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            //create an enemy at chose spawn location
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            //increase the amoun t of alive enemies
            enemiesAlive++;
        }

        waveNumber++;
        //each wave we are going to add two enemies
        //enemiesPerWave += 2;
    }

    IEnumerator SpawnWave()
    {
        while(true)//infinite loop bc always going to be true right now
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            SpawnEnemies();
        }
    }

    

    void EnemyDied()
    {
        //what happens when all the enemies die
        //dont actually just do die logic here do it in enemy AI
        //but do say if all enemies are dead WAVE CLEARED
    }
}
