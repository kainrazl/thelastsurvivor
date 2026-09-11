using UnityEngine;

[CreateAssetMenu(fileName = "NewGameItem", menuName = "Survivor/Game Item")]
public class ItemSO : ScriptableObject
{
    public ItemType type;
    public ItemRarity rarity;
    public float value;
    public AudioClip sound;
}
