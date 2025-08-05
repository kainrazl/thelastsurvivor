using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    private PlayerExp playerExp;
    private float expPoints = 5;

    private void Awake()
    {
        playerExp = GameObject.FindWithTag("Experience").GetComponent<PlayerExp>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerExp.UpdateExp(expPoints);
            Destroy(gameObject);
        }
    }
}
