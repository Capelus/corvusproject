using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrades Atlas", menuName = "Upgrades/New Upgrades Atlas", order = 0)]
public class UpgradesAtlas : ScriptableObject
{
    //ENGINES
    [Header("ENGINE UPGRADES")]
    public EngineUpgrade [] engineUpgrades;

    //CHASSIS
    [Header("CHASSIS UPGRADES")]
    public ChassisUpgrade[] chassisUpgrades;

    //JETS
    [Header("JET UPGRADES")]
    public JetUpgrade[] jetsUpgrades;

    //BLASTERS
    [Header("BLASTER UPGRADES")]
    public BlasterUpgrade[] blastersUpgrades;

    //TANKS
    [Header("TANK UPGRADES")]
    public TankUpgrade[] tanksUpgrades;

}
