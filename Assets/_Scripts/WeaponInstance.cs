using System;
using System.Collections;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    [SerializeField] private WeaponSO properties;
    private WeaponType weaponType;
    private Transform player;
    private float damage;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("EquipedWeapons").transform;
    }

    private void Start()
    {
        damage = properties.damage;

        weaponType = properties.type;

        switch (weaponType)
        {
            case WeaponType.Melee:
                break;

            case WeaponType.Throwable:
                break;

            case WeaponType.Circle:
                TryGetComponent(out SpinAttack spin);

                if (spin != null)
                {
                    spin.SetParent(player);
                    spin.SetRadius(properties.radius);
                    spin.SetRotationSpeed(properties.travelSpeed);
                }
                break;

            case WeaponType.Area:
                break;

            case WeaponType.Ranged:
                break;

            default:
                break;
        }
    }

    public WeaponSO GetProperties()
    {
        return properties;
    }
}