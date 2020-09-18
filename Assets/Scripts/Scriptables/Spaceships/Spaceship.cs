using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship", menuName = "ScriptableObjects/SpaceshipScriptableObject", order = 1)]
public class Spaceship : ScriptableObject
{
    [Header("Velocity")] //--------------------------------

    [Tooltip("The maximum velocity of the spaceship.")]
    public float maxSpeedValue = 120;
    //-----------------------------------------------------

    [Header("Acceleration")] //----------------------------

    [Tooltip("The maximum acceleration of the spaceship.")]
    public float maxAccelerationvalue = 40;

    [Tooltip("The curve defining the spaceship's acceleration. (Over velocity)")]
    public AnimationCurve accelerationCurve;

    [Tooltip("The curve defining the spaceship's deceleration. (Over velocity)")]
    public AnimationCurve brakeCurve;
    //-----------------------------------------------------

    [Header("Handling")] //--------------------------------

    [Tooltip("The maximum velocity the spaceship can move horizontally and vertically")]
    public float handlingValue = 15;

    [Tooltip("The curve defining the spaceship's handling. (Over velocity)")]
    public AnimationCurve handlingCurve;
    //-----------------------------------------------------

    [Header("Blaster")] //---------------------------------

    [Tooltip("The gameobject of the projectile to be shot by the player's spaceship")]
    public GameObject projectile;

    [Tooltip("The shoot ratio of the player's spaceship")]
    public float cadenceValue;
    //-----------------------------------------------------

    //[Header("Jets")] //------------------------------------

    //[Tooltip("The curve defining the spaceship's handling. (Over velocity)")]
    //public float boostValue;

    //[Tooltip("The curve defining the spaceship's handling. (Over velocity)")]
    //public float intakeValue;
    //-----------------------------------------------------
}






