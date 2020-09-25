using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jets Upgrade", menuName = "Upgrades/New Upgrade Component/ Jets Upgrade", order = 3)]
public class JetUpgrade : ScriptableObject
{
    //GENERAL INFO
    [Header("DATA")]
    public int id;
    public int unlockCost;
    public string upgradeName;
    public string upgradeInfo;

    [Header("VALUES")]
    //VALUES
    public float bonusMaxBoost;
    public float bonusMaxSuperBoost;
    public float bonusBoostIntake;
    public float bonusSuperBoostIntake;
}
