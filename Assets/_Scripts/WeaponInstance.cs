using System;
using System.Collections;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    [SerializeField] private WeaponSO properties;
    [SerializeField] private bool isUpgrade = false;
    private WeaponType weaponType;
    private Transform player;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("EquipedWeapons").transform;

        if (!isUpgrade) {
            weaponType = properties.type;

            switch (weaponType)
            {
                case WeaponType.Melee:
                    break;

                case WeaponType.Throwable:
                    break;

                case WeaponType.Circle:
                    TryGetComponent(out SpinRotation spin);

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
    }

    //public void EquipWeapon()
    //{
    //    transform.SetParent(player);
    //    transform.position = player.position;
    //}
}