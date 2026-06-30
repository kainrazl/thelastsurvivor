using UnityEngine;

[CreateAssetMenu(fileName = "New Interactable Element", menuName = "AZTEK/Interactable Element")]
public class InteractableElement : ScriptableObject
{
    public new string name;
    public string description;
    public GameObject prefab;
    public GameObject abilityPrefab;
}
