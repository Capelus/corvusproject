using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Engine Upgrade", menuName = "Upgrades/New Engine Upgrade", order = 1)]
public class EngineUpgrade : ScriptableObject
{
    //GENERAL INFO
    [Header("DATA")]
    public int id;
    public int unlockCost;
    public string upgradeName;
    public string upgradeInfo;

    [Header("VALUES")]
    //VALUES
    public float bonusMaxSpeed;
    public float bonusMaxAcceleration;
    public float bonusMaxBrake;
}
