using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Templates/Enemy", order = 1)]

public class Enemy : ScriptableObject
{
    public GameObject shipPrefab;
    public float health = 100;
    public GameObject projectile;
    public float cadence = 1;
    public float range = 10;
    public GameObject drop;
    public float dropAmount = 1;
}
