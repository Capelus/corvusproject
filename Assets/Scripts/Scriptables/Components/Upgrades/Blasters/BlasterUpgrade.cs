using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Blaster Upgrade", menuName = "Upgrades/New Blaster Upgrade", order = 4)]
public class BlasterUpgrade : ScriptableObject
{
    //GENERAL INFO
    [Header("DATA")]
    public int id;
    public int unlockCost;
    public string upgradeName;
    public string upgradeInfo;

    [Header("VALUES")]
    //VALUES
    public float bonusDamage;
    public float bonusCadence;
}
