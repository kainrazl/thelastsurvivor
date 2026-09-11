using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponManager : MonoBehaviour
{
    private string result = "";
    private bool upgradeFit = false; //To check if the upgrade fits on the selected weapon
    private UpgradeType upgradeType;
    private WeaponSO properties;
    private float damage;
    private float radius;
    private float speed;
    private float number;
    private float distance;
    private float coolDown;
    private float attackRate;
    
    public string SetUpgrade(WeaponType type)
    {
        upgradeType = (UpgradeType)Random.Range(1, 7);
        switch (type)
        {
            case WeaponType.Area:
                break;
            case WeaponType.Circle:
                while (!upgradeFit)
                {
                    if (upgradeType == UpgradeType.Number ||
                        upgradeType == UpgradeType.Damage ||
                        upgradeType == UpgradeType.Radius ||
                        upgradeType == UpgradeType.Speed)
                        upgradeFit = true;
                    else
                        upgradeType = (UpgradeType)Random.Range(1, 7);
                }
                
                SpinAttack[] circleWeapons = GetComponentsInChildren<SpinAttack>();
                
                foreach (SpinAttack weapon in circleWeapons)
                {
                    //Get weapon instance properties
                    properties = weapon.GetComponent<WeaponInstance>().GetProperties();
                    
                    switch (upgradeType)
                    {
                        case UpgradeType.Number:
                            int rounds = weapon.GetRounds() + 1;
                            
                            if (rounds <= properties.maxNumberOfRounds)
                                weapon.SetRounds(rounds);
                            
                            break;
                        case UpgradeType.Damage:
                            damage = weapon.GetDamage() + 0.5f;
                            
                            if (damage <= properties.maxDamage)
                                weapon.SetDamage(damage);
                            
                            break;
                        case UpgradeType.Radius:
                            radius = weapon.GetRadius() + 0.3f;
                            
                            if (radius <= properties.maxRadius)
                                weapon.SetRadius(radius);
                            break;
                        case UpgradeType.Speed:
                            speed = weapon.GetRotationSpeed() + 0.5f;
                            
                            if (speed <= properties.maxTravelSpeed)
                                weapon.SetRotationSpeed(3);
                            break;
                    }
                }

                break;
            case WeaponType.Melee: //Always will be 1 melee weapon
                MeleeAttack meleeWeapon = GetComponentInChildren<MeleeAttack>();
                
                //Get weapon instance properties
                properties = meleeWeapon.GetComponent<WeaponInstance>().GetProperties();

                damage = meleeWeapon.GetDamage();
                
                if(damage <= properties.maxDamage)
                    meleeWeapon.SetDamage(3);
                
                break;
            case WeaponType.Ranged:
                break;
            case WeaponType.Throwable:
                break;
        }
        
        return result;
    }
}