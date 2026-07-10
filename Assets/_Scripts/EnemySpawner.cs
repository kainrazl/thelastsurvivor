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
    public EnemySpawnConfig[] enemyConfigs; // enemiesPrefabs y tiempo de aparición
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
    public float maxSpawnTime;
    private int waveCounter = 0;
    private bool increaseEnemyProperties = false;

    public void Start()
    {
        maxSpawnTime = 0;

        foreach (EnemySpawnConfig config in enemyConfigs)
        {
            if (config.maxTimeToAppear > maxSpawnTime)
            {
                maxSpawnTime = config.maxTimeToAppear;
            }
        }
    }

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
            GameObject newEnemy = Instantiate(enemyToSpawn, randomSpawnPoint.position, Quaternion.identity);
            //SpriteRenderer sr = newEnemy.GetComponent<SpriteRenderer>();
            // sr.sortingLayerName = "Elements";
            if (increaseEnemyProperties)
            {
                UpdateEnemyProperties(newEnemy);
            }

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
        }else
            //Actualiza la configuración de los enemigos si el tiempo transcurrido supera el tiempo máximo de aparición
            if(elapsedTime > maxSpawnTime)
                UpdateEnemyConfigs();

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

    private void UpdateEnemyConfigs()
    {
        float newMaxSpawnTime = 0;
        maxSpawnTime += 30f; // Incrementa el tiempo máximo en 30 segundos por el último jefe generado

        //Se actualizan los tiempos de aparición de los enemigos en la lista de configuraciones
        foreach (EnemySpawnConfig config in enemyConfigs)
        {
            config.minTimeToAppear += maxSpawnTime;
            config.maxTimeToAppear += maxSpawnTime;
            newMaxSpawnTime = config.maxTimeToAppear;
        }
        maxSpawnTime = newMaxSpawnTime;
        waveCounter++;
        increaseEnemyProperties = true;
    }

    private void UpdateEnemyProperties(GameObject enemy)
    {
        //Incrementa las propiedades del enemigo en cierto porcentaje según las waves generadas
        float healthMultiplier = 0.3f * waveCounter;
        float damageMultiplier = 0.1f * waveCounter;
        float minSpeedMultiplier = 0.05f * waveCounter;
        float maxSpeedMultiplier = 0.05f * waveCounter;

        enemy.TryGetComponent(out EnemyManager enemyManager);
        if (enemyManager != null)
        {
            CreatureProperties updatedProperties = enemyManager.GetEnemyProperties();
            updatedProperties.health = updatedProperties.health * (1 + healthMultiplier);
            updatedProperties.damage = updatedProperties.damage * (1 + damageMultiplier);
            updatedProperties.minSpeed = updatedProperties.minSpeed * (1 + minSpeedMultiplier);
            updatedProperties.maxSpeed = updatedProperties.maxSpeed * (1 + maxSpeedMultiplier);

            enemyManager.SetEnemyProperties(updatedProperties);
        }
    }
}
