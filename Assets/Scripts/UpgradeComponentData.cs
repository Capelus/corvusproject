using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeComponentData : MonoBehaviour
{
    //REFERENCES
    public WorkshopManager workshopManager;
    Button button;
    public Text label;

    //COMPONENT DATA
    public string upgradeType;
    public ScriptableObject upgradeComponentData;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ChangeComponent);
    }

    void ChangeComponent()
    {
        switch (upgradeType)
        {
            case "Engine":
                workshopManager.ChangeEngineUpgrade((EngineUpgrade)upgradeComponentData);
                break;

            case "Chassis":
                workshopManager.ChangeChassisUpgrade((ChassisUpgrade)upgradeComponentData);
                break;

            case "Jet":
                workshopManager.ChangeJetUpgrade((JetUpgrade)upgradeComponentData);
                break;

            case "Blaster":
                workshopManager.ChangeBlasterUpgrade((BlasterUpgrade)upgradeComponentData);
                break;

            case "Tank":
                workshopManager.ChangeTankUpgrade((TankUpgrade)upgradeComponentData);
                break;
        }
    }
}
