using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship Profile", menuName = "Spaceship/New Spaceship Profile", order = 1)]
public class SpaceshipProfile : ScriptableObject
{
    [Header("Engine")]
    public EngineProfile engineProfile;

    [Header("Chassis")]
    public ChassisProfile chassisProfile;

    [Header("Blasters")]
    public BlasterProfile blasterProfile;

    [Header("Jets")]
    public JetProfile jetProfile;

    [Header("Tank")]
    public TankProfile tankProfile;
}






