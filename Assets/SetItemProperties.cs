using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetItemProperties : MonoBehaviour
{
    [SerializeField] private Item properties;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = properties.sound;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        audioSource.Play();
        gameObject.SetActive(false);
        Destroy(gameObject, properties.sound.length);
    }
}
