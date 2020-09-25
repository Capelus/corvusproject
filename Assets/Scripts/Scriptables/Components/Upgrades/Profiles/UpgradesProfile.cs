using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrades Profile", menuName = "Upgrades/New Upgrade Profile", order = 6)]
public class UpgradesProfile : ScriptableObject
{
    //ENGINE
    [Header("ENGINE UPGRADE")]
    public EngineUpgrade engineUpgrades;

    //CHASSIS
    [Header("CHASSIS UPGRADE")]
    public ChassisUpgrade chassisUpgrades;

    //JETS
    [Header("JET UPGRADE")]
    public JetUpgrade jetsUpgrades;

    //BLASTER
    [Header("BLASTER UPGRADE")]
    public BlasterUpgrade blastersUpgrades;

    //TANK
    [Header("TANK UPGRADE")]
    public TankUpgrade tanksUpgrades;
}
