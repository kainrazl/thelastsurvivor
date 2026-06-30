using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EnemySpawnConfig
{
    public GameObject prefab;
    public float minTimeToAppear = 0f;  // Tiempo mínimo (en segundos)
    public float maxTimeToAppear = Mathf.Infinity;  // valor opcional: tiempo máximo
}

public class EnemySpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public EnemySpawnConfig[] enemyConfigs;  // enemiesPrefabs y tiempo de aparición
    public GameObject player;
    public int enemyCounter = 0;

    [SerializeField] private float timeBtwSpawn = 3f;
    [SerializeField] private float minTimeBtwSpawn = 0.5f;
    [SerializeField] private float timeBtwIncreaseRate = 30f;
    [SerializeField] private int spawnRate = 6;
    [SerializeField] private int maxEnemies = 100;

    private int spawnNumber = 0;    
    private bool canSpawn = true;
    private bool canIncrement = true;
    private float elapsedTime = 0f;
    private bool isBossAlreadySpawned = false;

    public void SetenemyCounter()
    {
        enemyCounter--;
    }

    public void SetBossSpawned(bool value)
    {
        isBossAlreadySpawned = value;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (canIncrement && spawnNumber < maxEnemies)
        {
            StartCoroutine(IncreaseSpawnRate());
        }

        if (player != null && canSpawn && enemyCounter < spawnNumber)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    private IEnumerator SpawnEnemies()
    {
        canSpawn = false;

        WaitForSeconds waiting = new WaitForSeconds(timeBtwSpawn);
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyToSpawn = GetEnemyByTime();

        if (enemyToSpawn != null)
        {
            Instantiate(enemyToSpawn, randomSpawnPoint.position, Quaternion.identity);
            SpriteRenderer sr = enemyToSpawn.GetComponent<SpriteRenderer>();
            sr.sortingLayerName = "Elements";
            //sr.sortingOrder = 1;

            enemyCounter += 1;
        }

        yield return waiting;
        canSpawn = true;
    }

    private GameObject GetEnemyByTime()
    {
        // Filtra enemigos disponibles según el tiempo transcurrido
        List<EnemySpawnConfig> availableEnemies = new List<EnemySpawnConfig>();

        foreach (EnemySpawnConfig config in enemyConfigs)
        {
            if (elapsedTime >= config.minTimeToAppear && (config.maxTimeToAppear == 0 || elapsedTime <= config.maxTimeToAppear))
            {
                if (config.prefab.GetComponent<EnemyManager>().GetEnemyProperties().isBoss)
                {
                    if(!isBossAlreadySpawned)
                    {
                        availableEnemies.Clear(); // Limpiar la lista de enemigos disponibles
                        isBossAlreadySpawned = true; // Marca que el jefe ya ha sido generado
                        return config.prefab; // Instanciar boss enemy
                    }
                    else
                    {
                        continue; // Si el jefe ya ha sido generado, no agregarlo a la lista de enemigos disponibles
                    }
                }
                else
                {
                    availableEnemies.Add(config);
                }
            }
        }

        if (availableEnemies.Count > 0)
        {
            return availableEnemies[Random.Range(0, availableEnemies.Count)].prefab;
        }

        return null;
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
