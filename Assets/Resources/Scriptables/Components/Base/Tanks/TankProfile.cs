using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tank Profile", menuName = "Components/New Tank Profile", order = 5)]
public class TankProfile : ScriptableObject
{
    //----------------------------------- ENERGY
    [Tooltip("The total capacity of the energy tank.")]
    public float capacity;
}
