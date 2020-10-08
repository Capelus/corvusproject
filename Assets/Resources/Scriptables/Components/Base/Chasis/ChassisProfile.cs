using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chassis Profile", menuName = "Components/New Chassis Profile", order = 2)]
public class ChassisProfile : ScriptableObject
{
    //--------------------------------- LIMIT SPEED
    [Tooltip("The maximum velocity the spaceship can reach.")]
    public float limitSpeed = 500;

    //----------------------------------- HANDLING
    [Tooltip("The maximum velocity the spaceship can move horizontally and vertically.")]
    public float handling = 15;

    [Tooltip("The curve defining the spaceship's handling. (Over velocity)")]
    public AnimationCurve handlingCurve;

    //--------------------------------- RESISTANCE
    [Tooltip("The amount of deceleration after crashing on an obstacle.")]
    public float knockback = 10;
}
