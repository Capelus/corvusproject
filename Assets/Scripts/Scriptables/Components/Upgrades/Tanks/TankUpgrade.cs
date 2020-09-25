using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tank Upgrade", menuName = "Upgrades/New Upgrade Component/Tank Upgrade", order = 5)]
public class TankUpgrade : ScriptableObject
{
    //GENERAL INFO
    [Header("DATA")]
    public int id;
    public int unlockCost;
    public string upgradeName;
    public string upgradeInfo;


    [Header("VALUES")]
    //VALUES
    public float bonusCapacity;
}
