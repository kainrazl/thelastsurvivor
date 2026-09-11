using UnityEngine;

[System.Serializable]
public class WeaponSO
{
    public WeaponType type;
    [Tooltip("Damage dealt by this weapon")] public float damage;
    [Tooltip("Attack rate of this weapon")] public float attackRate;
    [Tooltip("Duration of this weapon's effect")] public float duration;
    [Tooltip("Cooldown period for this weapon")] public float coolDown;
    [Tooltip("Radius of this weapon's area of effect")] public float radius;
    [Tooltip("Distance this weapon can reach")] public float distance;
    [Tooltip("Speed at which this weapon travels")] public float travelSpeed;
    [Tooltip("Number of rounds this weapon can fire")] public int numberOfRounds;
    [Tooltip("Name of this weapon")] public string name;

    //Upgrade parameters
    [Tooltip("Maximum damage this weapon can deal")] public float maxDamage;
    [Tooltip("Maximum attack rate this weapon can have")] public float maxAttackRate;
    [Tooltip("Maximum duration this weapon's effect can last")] public float maxDuration;
    [Tooltip("Maximum cooldown period for this weapon")] public float maxCoolDown;
    [Tooltip("Maximum radius of this weapon's area of effect")] public float maxRadius;
    [Tooltip("Maximum distance this weapon can reach")] public float maxDistance;
    [Tooltip("Maximum number of rounds this weapon can fire")] public int maxNumberOfRounds;
    [Tooltip("Maximum speed at which this weapon can travel")] public float maxTravelSpeed;
    [Tooltip("Maximum number of weapons the player can equip")] public int maxWeapons;
}