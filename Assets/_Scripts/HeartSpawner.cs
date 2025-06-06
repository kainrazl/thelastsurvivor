using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{    
    public int heartCount = 0;

    [SerializeField] private GameObject heartPrefab;
    private int maxHearts = 4;
    private bool canSpawn = true;
    private float timeBtwSpawn = 30f;

    private void Update()
    {
        if (canSpawn && heartCount < maxHearts)
        {
            StartCoroutine(SpawnHeart());
        }
    }

    private IEnumerator SpawnHeart()
    {
        canSpawn = false;

        WaitForSeconds waiting = new WaitForSeconds(timeBtwSpawn);

        float positionX = Random.Range(-7, 7);
        float positionY = Random.Range(-4, 3.6f);
        Vector3 position = new Vector3(positionX, positionY, 0);

        Instantiate(heartPrefab, position, Quaternion.identity);
        SpriteRenderer sr = heartPrefab.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Elements";
        sr.sortingOrder = 1;
        heartCount++;

        yield return waiting;

        canSpawn = true;
    }
}
