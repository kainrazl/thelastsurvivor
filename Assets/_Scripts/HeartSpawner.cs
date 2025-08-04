using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{    
    public int heartCount = 0;

    [SerializeField] private GameObject heartPrefab;
    private int maxHearts = 2;
    private bool canSpawn = false;
    private float timeBtwSpawn = 9f; //time for item to drop
    private float lastSpawn = 0;
    private List<DropItem> listItems = new List<DropItem>();
    [SerializeField] private GameObject[] itemPrefabs;

    private void Awake()
    {
        foreach (GameObject prefab in itemPrefabs)
        {
            string name = prefab.name;
            DropItem item;

            item = new();
            item.prefab = prefab;

            switch (name)
            {
                case "lollipop":
                    item.rarity = ItemRarity.Rare;
                    item.type = ItemType.Experience;
                    item.value = 5f;
                    break;

                case "candy":
                    item.rarity = ItemRarity.Common;
                    item.type = ItemType.Experience;
                    item.value = 2f;
                    break;

                case "Heart":
                    item.rarity = ItemRarity.UltraRare;
                    item.type = ItemType.Health;
                    item.value = 0.2f;
                    break;

                default:
                    break;
            }
            listItems.Add(item);
        }
    }

    private void Update()
    {
        lastSpawn += Time.deltaTime;
        canSpawn = /*heartCount < maxHearts &&*/ lastSpawn >= timeBtwSpawn;
    }

    private IEnumerator AutomaticHeartSpawn()
    {
        canSpawn = false;

        WaitForSeconds waiting = new WaitForSeconds(timeBtwSpawn);

        float positionX = UnityEngine.Random.Range(-7, 7);
        float positionY = UnityEngine.Random.Range(-4, 3.6f);
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
            //foreach(DropItem item in listItems)
            //{

            //}
            try
            {
                int prefabIndex = UnityEngine.Random.Range(0, listItems.Count);
                GameObject prefab = listItems[prefabIndex].prefab;

                Vector3 position = new Vector3(positionX, positionY, 0);
                //Instantiate(heartPrefab, position, Quaternion.identity);
                //SpriteRenderer sr = heartPrefab.GetComponent<SpriteRenderer>();
                Instantiate(prefab, position, Quaternion.identity);
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Elements";
                sr.sortingOrder = 1;
                heartCount++;
                lastSpawn = 0;
            }
            catch (AndroidJavaException e)
            {
                Debug.Log(e.Message);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}
