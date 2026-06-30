using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{    
    public int itemCount = 0;

    [SerializeField] private List<GameObject> listItems;

    public int maxItems = 30; //max number of items that can be spawned at a time
    public float timeBtwSpawn = 1.5f; //time in seconds for item to drop
    private bool canSpawn = false;
    private float lastSpawn = 0;

    private void Update()
    {
        lastSpawn += Time.deltaTime;
        canSpawn = itemCount < maxItems && (lastSpawn >= timeBtwSpawn);
    }

    // private IEnumerator AutomaticItemSpawn()
    // {
    //     canSpawn = false;

    //     WaitForSeconds waiting = new WaitForSeconds(timeBtwSpawn);

    //     float positionX = UnityEngine.Random.Range(-7, 7);
    //     float positionY = UnityEngine.Random.Range(-4, 3.6f);
    //     // Vector3 position = new Vector3(positionX, positionY, 0);
    //     itemCount++;

    //     yield return waiting;

    //     canSpawn = true;
    // }

    public void GetItem(float positionX, float positionY)
    {
        if (canSpawn)
        {
            //foreach(ItemProperties item in listItems)
            //{

            //}
            try
            {
                int prefabIndex = 0;
                
                // Calcular el peso total basado en la rareza invertida (1/rarity)
                float totalWeight = 0f;
                foreach (GameObject item in listItems)
                {
                    ItemProperties itemProps = item.GetComponent<ItemEffect>().properties;
                    if (itemProps != null && itemProps.rarity > 0)
                        totalWeight += 1f / ((float)itemProps.rarity);
                }

                // Seleccionar un item basado en su peso inverso de rareza
                float randomValue = UnityEngine.Random.Range(0f, totalWeight);
                float currentWeight = 0f;

                for (int i = 0; i < listItems.Count; i++)
                {
                    ItemProperties itemProps = listItems[i].GetComponent<ItemEffect>().properties;
                    if (itemProps != null && itemProps.rarity > 0)
                    {
                        currentWeight += 1f / ((float)itemProps.rarity);
                        if (randomValue <= currentWeight)
                        {
                            prefabIndex = i;
                            break;
                        }
                    }
                }

                GameObject prefab = listItems[prefabIndex];

                Vector3 position = new Vector3(positionX, positionY, 0);
                Instantiate(prefab, position, Quaternion.identity);
                SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
                sr.sortingLayerName = "Elements";
                //sr.sortingOrder = 1;
                itemCount++;
                lastSpawn = 0;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
