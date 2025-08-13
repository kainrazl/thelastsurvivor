using UnityEngine;

[System.Serializable]
public class ItemProperties
{
    public ItemType type;
    public ItemRarity rarity;
    public float value;
    public AudioClip sound = null;
}

public enum ItemType
{
    Health = 1,
    Damage = 2,
    Experience = 3
}

public enum ItemRarity
{
    Common = 1,
    Normal = 2,
    Rare = 3,
    UltraRare = 4
}