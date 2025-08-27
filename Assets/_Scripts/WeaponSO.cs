using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Survivor/Weapon")]
public class WeaponSO : ScriptableObject
{
    public WeaponType type;
    public float damage;
    public float attackRate;
    public float duration;
    public float coolDown;
    public float radius;
    public float distance;
    public float travelSpeed;
    public new string name;
    public string description;
}