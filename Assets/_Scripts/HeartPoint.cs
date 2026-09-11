using UnityEngine;

public class HeartPoint : MonoBehaviour
{
    private ItemSpawner itemSpawner;
    private AudioSource sound;
    private PlayerHealth ph;
    private float playerCurrentHealth;

    private void Start()
    {
        itemSpawner = GameObject.Find("HeartSpawner").GetComponent<ItemSpawner>();
        sound = GameObject.Find("HeartSound").GetComponent<AudioSource>();
        ph = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        playerCurrentHealth = ph.currentHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerCurrentHealth < 1)
        {
                sound.Play();
                Destroy(gameObject);
                itemSpawner.itemCount--;
        }
    }
}
