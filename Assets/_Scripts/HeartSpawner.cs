using System.Collections;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{    
    public int heartCount = 0;

    [SerializeField] private GameObject heartPrefab;
    private int maxHearts = 2;
    private bool canSpawn = false;
    private float timeBtwSpawn = 30f;
    private float lastSpawn = 0;

    private void Update()
    {
        lastSpawn += Time.deltaTime;

        canSpawn = heartCount < maxHearts && lastSpawn >= timeBtwSpawn;
    }

    private IEnumerator AutomaticHeartSpawn()
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

    public void GetHeart(float positionX, float positionY)
    {
        if (canSpawn)
        {
            Vector3 position = new Vector3(positionX, positionY, 0);
            Instantiate(heartPrefab, position, Quaternion.identity);
            SpriteRenderer sr = heartPrefab.GetComponent<SpriteRenderer>();
            sr.sortingLayerName = "Elements";
            sr.sortingOrder = 1;
            heartCount++;
            lastSpawn = 0;
        }
    }
}
