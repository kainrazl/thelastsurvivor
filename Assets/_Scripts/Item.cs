using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "New Item")]
public class Item : ScriptableObject
{
    public ItemType type;
    public ItemRarity rarity;
    public float value;
    public AudioClip sound;
}
