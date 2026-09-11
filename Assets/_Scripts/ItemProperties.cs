using UnityEngine;

[System.Serializable]
public class ItemProperties
{
    public ItemType type;
    public ItemRarity rarity;
    public float value;
    public AudioClip sound = null;
}