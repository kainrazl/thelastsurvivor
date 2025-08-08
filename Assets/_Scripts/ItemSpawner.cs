using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{    
    public int itemCount = 0;

    [SerializeField] private List<GameObject> listItems;

    public int maxItems = 3;
    public float timeBtwSpawn = 9f; //time in seconds for item to drop
    private bool canSpawn = false;
    private float lastSpawn = 0;

    private void Update()
    {
        lastSpawn += Time.deltaTime;
        canSpawn = itemCount < maxItems && (lastSpawn >= timeBtwSpawn);
    }

    private IEnumerator AutomaticItemSpawn()
    {
        canSpawn = false;

        WaitForSeconds waiting = new WaitForSeconds(timeBtwSpawn);

        float positionX = UnityEngine.Random.Range(-7, 7);
        float positionY = UnityEngine.Random.Range(-4, 3.6f);
        Vector3 position = new Vector3(positionX, positionY, 0);

        //Instantiate(heartPrefab, position, Quaternion.identity);
        //SpriteRenderer sr = heartPrefab.GetComponent<SpriteRenderer>();
        //sr.sortingLayerName = "Elements";
        //sr.sortingOrder = 1;
        itemCount++;

        yield return waiting;

        canSpawn = true;
    }

    public void GetItem(float positionX, float positionY)
    {
        if (canSpawn)
        {
            //foreach(ItemProperties item in listItems)
            //{

            //}
            try
            {
                int prefabIndex = UnityEngine.Random.Range(0, listItems.Count);
                GameObject prefab = listItems[prefabIndex];

                Vector3 position = new Vector3(positionX, positionY, 0);
                Instantiate(prefab, position, Quaternion.identity);
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Elements";
                sr.sortingOrder = 1;
                itemCount++;
                lastSpawn = 0;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}
