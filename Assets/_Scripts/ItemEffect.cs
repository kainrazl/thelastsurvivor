using System;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    [SerializeField] ItemProperties properties;
    private ItemSpawner itemSpawner;
    private AudioSource audioSource;

    private void Awake()
    {
        itemSpawner = FindAnyObjectByType<ItemSpawner>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            PlayerExp playerExp = player.GetComponent<PlayerExp>();
            ItemType itemType = properties.type;
            float points = properties.value;

            ItemSound();

            switch (itemType)
            {
                case ItemType.Health:
                    if (playerHealth.currentHealth < 1)
                        playerHealth.UpdateHealth(points, false);
                    
                    break;

                case ItemType.Damage:
                    playerHealth.UpdateHealth(points, true);
                    break;

                case ItemType.Experience:
                    playerExp.UpdateExp(points);
                    break;

                default:
                    break;
            }

            itemSpawner.itemCount--;
            Destroy(gameObject);
        }
    }

    private void ItemSound()
    {
        try
        {
            //audioSource.volume = 0.3f;
            audioSource.Play();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
