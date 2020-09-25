using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chassis Upgrade", menuName = "Upgrades/New Upgrade Component/Chassis Upgrade", order = 2)]
public class ChassisUpgrade : ScriptableObject
{
    //GENERAL INFO
    [Header("DATA")]
    public int id;
    public int unlockCost;
    public string upgradeName;
    public string upgradeInfo;

    [Header("VALUES")]
    //VALUES
    public float bonusMaxHandling;
    public float bonusKnockback;
}
