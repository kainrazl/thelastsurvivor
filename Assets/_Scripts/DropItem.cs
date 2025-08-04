using UnityEngine;

public struct DropItem
{
    public GameObject prefab { get; set; }
    public ItemType type { get; set; }
    public ItemRarity rarity { get; set; }
    public float value { get; set; }
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