using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Survivor/Weapon")]
public class WeaponSO : ScriptableObject
{
    //Active parameters
    public WeaponType type;
    public float damage;
    public float attackRate;
    public float duration;
    public float coolDown;
    public float radius;
    public float distance;
    public float travelSpeed;
    public int numberOfRounds;
    public new string name;
    
    //Upgrade parameters
    public float maxDamage;
    public float maxAttackRate;
    public float maxDuration;
    public float maxCoolDown;
    public float maxRadius;
    public float maxDistance;
    public int maxNumberOfRounds;
    public float maxTravelSpeed;
}