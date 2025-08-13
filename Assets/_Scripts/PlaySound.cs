using System;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void MakeSound(AudioClip sound)
    {
        try
        {
            audioSource.volume = 0.3f;
            audioSource.PlayOneShot(sound);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
