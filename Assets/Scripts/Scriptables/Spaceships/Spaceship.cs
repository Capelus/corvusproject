﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship", menuName = "ScriptableObjects/SpaceshipScriptableObject", order = 1)]
public class Spaceship : ScriptableObject
{
    
    public Mesh spaceshipMesh;
    public Rigidbody rb;
    //MAIN ATTRIBUTES
   
    public float speedValue;
    public float accelerationvalue;
    public float sizeValue;
    public float resistanceValue;
    public float handlingValue;
    public float projectileValue;
    public float powerValue;
    public float cadenceValue;
    public float boostValue;
    public float intakeValue;

    //ENGINE ATTRIBUTES

    [Range(0,10)]public int speedLevel;
    [Range(0, 10)] public int accelerationLevel;

    //CHASSIS ATTRIBUTES

    [Range(0, 10)] public int resistanceLevel;
    [Range(0, 10)] public int handlingLevel;

    //BLASTERS ATTRIBUTES

    [Range(0, 10)] public int blasterType;
    [Range(0, 10)] public int powerLevel;
    [Range(0, 10)] public int cadenceLevel;

    //TURBO ATTRIBUTES

    [Range(0, 10)] public int boostLevel;
    [Range(0, 10)] public int intakeLevel;

    
}





