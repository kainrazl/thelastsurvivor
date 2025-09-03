using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Survivor/Enemy")]
public class EnemySO : ScriptableObject
{
    new public string name;
    public string description;
    public float health;
    public float minSpeed;
    public float maxSpeed;
    public float damage;
}
