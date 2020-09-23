using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blaster Profile", menuName = "Components/New Blaster Profile", order = 3)]
public class BlasterProfile : ScriptableObject
{
    //----------------------------------- PROJECTILE
    [Tooltip("The gameobject of the projectile to be shot by the player's spaceship.")]
    public GameObject projectile;

    //-------------------------------------- CADENCE
    [Tooltip("The shoot ratio of the player's spaceship.")]
    public float cadence;
}
