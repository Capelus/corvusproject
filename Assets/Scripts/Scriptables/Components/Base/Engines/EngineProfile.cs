using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Engine Profile", menuName = "Components/New Engine Profile", order = 1)]
public class EngineProfile : ScriptableObject
{
    //----------------------------------- VELOCITY
    [Header("Velocity")]
    [Tooltip("The maximum velocity of the spaceship.")]
    public float maxSpeed = 120;

    //------------------------------- ACCELERATION
    [Header("Acceleration")]
    [Tooltip("The maximum acceleration of the spaceship.")]
    public float maxAcceleration = 40;

    [Tooltip("The curve defining the spaceship's acceleration. (Over velocity)")]
    public AnimationCurve accelerationCurve;

    //-------------------------------------- BRAKE
    [Header("Brake")]
    [Tooltip("The maximum deceleration of the spaceship.")]
    public float maxBrake = 40;

    [Tooltip("The curve defining the spaceship's deceleration. (Over velocity)")]
    public AnimationCurve brakeCurve;
}
