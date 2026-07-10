using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Creature", menuName = "Survivor/Creature")]
[System.Serializable]
public class CreatureProperties
{
    public string name;
    public string description;
    public float health;
    public float minSpeed;
    public float maxSpeed;
    public float damage;
    public bool isBoss;
    public GameObject bossItemPrefab;
}
