using System;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    [SerializeField] ItemProperties properties;
    private ItemSpawner itemSpawner;
    private AudioClip sound;
    private PlaySound play;

    private void Awake()
    {
        itemSpawner = FindAnyObjectByType<ItemSpawner>();
        play = FindAnyObjectByType<PlaySound>();
        sound = properties.sound;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            play.MakeSound(sound); //To make a sound when item is touched
            GameObject player = collision.gameObject;
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            PlayerExp playerExp = player.GetComponent<PlayerExp>();
            ItemType itemType = properties.type;
            float points = properties.value;

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
}
