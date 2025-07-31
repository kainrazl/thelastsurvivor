using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] enemiesPrefabs;
    public GameObject player;
    public int enemiesCounter = 0;

    [SerializeField] private float timeBtwSpawn = 5f;
    [SerializeField] private float minTimeBtwSpawn = 0.5f;
    [SerializeField] private float timeBtwIncreaseRate = 12f;
    [SerializeField] private int spawnRate = 6;
    [SerializeField] private int maxEnemies = 100;

    private int spawnNumber = 0;    
    private bool canSpawn = true;
    private bool canIncrement = true;

    public void SetEnemiesCounter()
    {
        enemiesCounter--;
    }

    private void Update()
    {
        if (canIncrement && spawnNumber < maxEnemies)
        {
            StartCoroutine(IncreaseSpawnRate());
        }

        if (player != null && canSpawn && enemiesCounter < spawnNumber)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        canSpawn = false;

        WaitForSeconds waiting = new WaitForSeconds(timeBtwSpawn);
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject randomObject = enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)];

        Instantiate(randomObject, randomSpawnPoint.position, Quaternion.identity);
        SpriteRenderer sr = randomObject.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Elements";
        sr.sortingOrder = 1;

        enemiesCounter += 1;

        yield return waiting;

        canSpawn = true;
    }

    private IEnumerator IncreaseSpawnRate()
    {
        canIncrement = false;

        WaitForSeconds waiting = new WaitForSeconds(timeBtwIncreaseRate);
        spawnNumber += spawnRate;
        if (timeBtwSpawn > minTimeBtwSpawn)
            timeBtwSpawn -= 0.5f;
        yield return waiting;

        canIncrement = true;
    }
}
