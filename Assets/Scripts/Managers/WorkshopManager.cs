using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class WorkshopManager : MonoBehaviour
{
    //REFERENCES
    public GameObject upgradeInterface;
    public GameObject selectionInterface;

    public Transform engineSection;
    public Transform chassisSection;
    public Transform jetSection;
    public Transform blasterSection;
    public Transform tankSection;

    Camera cam;

    //SPACESHIPS
    List<GameObject> spaceships = new List<GameObject>();
    public Transform garage;

    //UPGRADE BUTTONS
    public GameObject upgradeButton;
    GameObject [,,] upgradeButtons;

    //SELECTION
    int spaceshipCount;
    GameObject currentSpaceship;

    //VARIABLES
    public float viewRotationSpeed = 5;
    Transform cameraTarget;

    void Start()
    {
        //GET SPACESHIPS FROM GARAGE
        foreach (Transform s in garage.transform)
        {
            spaceships.Add(s.gameObject);
        }
        currentSpaceship = spaceships[0];

        //CREATE UPGRADE BUTTONS
        upgradeButtons = new GameObject[spaceships.Count, 5, 5];
        
        //ITERATE SPACESHIPS
        for (int i = 0; i < spaceships.Count; i++){
            
            //ITERATE COMPONENT CATEGORIES (ENGINE, CHASSIS...)
            for (int j = 0; j < 5; j++)
            {
                //ITERATE FOR EACH UPGRADE AVAILABLE
                for(int k = 0; k < 3; k++) //DE MOMENTO ASI DE HARDCODED
                {
                    switch (j)
                    {
                        case 0: //ENGINE
                            upgradeButtons[i, j, k] = Instantiate(upgradeButton, engineSection.position, Quaternion.identity);
                            upgradeButtons[i, j, k].transform.SetParent(engineSection, false);
                            upgradeButtons[i, j, k].transform.localScale = new Vector3(1, 1, 1);
                            upgradeButtons[i, j, k].transform.position = engineSection.position + engineSection.transform.right * 2 * (k + 1);
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeComponentData = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.engineUpgrades[k];
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeType = "Engine";
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().label.text = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.engineUpgrades[k].upgradeName;
                            break;

                        case 1: //CHASSIS
                            upgradeButtons[i, j, k] = Instantiate(upgradeButton, chassisSection.position + new Vector3(2, 0, 0) * k, Quaternion.identity);
                            upgradeButtons[i, j, k].transform.SetParent(chassisSection, false);
                            upgradeButtons[i, j, k].transform.localScale = new Vector3(1, 1, 1);
                            upgradeButtons[i, j, k].transform.position = chassisSection.position + chassisSection.transform.right * 2 * (k + 1);
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeComponentData = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.chassisUpgrades[k];
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeType = "Chassis";
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().label.text = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.chassisUpgrades[k].upgradeName;
                            break;

                        case 2: //JET
                            upgradeButtons[i, j, k] = Instantiate(upgradeButton, jetSection.position + new Vector3(2, 0, 0) * k, Quaternion.identity);
                            upgradeButtons[i, j, k].transform.SetParent(jetSection, false);
                            upgradeButtons[i, j, k].transform.localScale = new Vector3(1, 1, 1);
                            upgradeButtons[i, j, k].transform.position = jetSection.position + jetSection.transform.right * 2 * (k + 1);
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeComponentData = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.jetsUpgrades[k];
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeType = "Jet";
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().label.text = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.jetsUpgrades[k].upgradeName;
                            break;

                        case 3: //BLASTER
                            upgradeButtons[i, j, k] = Instantiate(upgradeButton, blasterSection.position + new Vector3(2, 0, 0) * k, Quaternion.identity);
                            upgradeButtons[i, j, k].transform.SetParent(blasterSection, false);
                            upgradeButtons[i, j, k].transform.localScale = new Vector3(1, 1, 1);
                            upgradeButtons[i, j, k].transform.position = blasterSection.position + blasterSection.transform.right * 2 * (k + 1);
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeComponentData = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.blastersUpgrades[k];
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeType = "Blaster";
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().label.text = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.blastersUpgrades[k].upgradeName;
                            break;

                        case 4: //TANK
                            upgradeButtons[i, j, k] = Instantiate(upgradeButton, tankSection.position + new Vector3(2, 0, 0) * k, Quaternion.identity);
                            upgradeButtons[i, j, k].transform.SetParent(tankSection, false);
                            upgradeButtons[i, j, k].transform.localScale = new Vector3(1, 1, 1);
                            upgradeButtons[i, j, k].transform.position = tankSection.position + tankSection.transform.right * 2 * (k + 1);
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeComponentData = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.tanksUpgrades[k];
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().upgradeType = "Tank";
                            upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().label.text = spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.tanksUpgrades[k].upgradeName;
                            break;
                    }

                    upgradeButtons[i, j, k].GetComponent<UpgradeComponentData>().workshopManager = this;
                }             
            }       
        }

        HideAllUpgrades();

        //INITIALIZE REFERENCES
        cam = Camera.main;
        cameraTarget = currentSpaceship.transform;
    }

    void Update()
    {
        //ROTATE SPACESHIP
        currentSpaceship.transform.Rotate(transform.up, -Input.GetAxis("Horizontal") * viewRotationSpeed * Time.deltaTime);

        //LOOK AT TARGET
        var targetRotation = Quaternion.LookRotation(cameraTarget.position - cam.transform.position);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, 20 * Time.deltaTime);
    }

    public void NextSpaceship()
    {
        //LOOP SELECTION
        if(spaceshipCount + 1 > spaceships.Count - 1)
        {
            currentSpaceship.SetActive(false);
            spaceshipCount = 0;
            currentSpaceship = spaceships[spaceshipCount];
            currentSpaceship.SetActive(true);
        }

        else
        {
            currentSpaceship.SetActive(false);
            spaceshipCount++;
            currentSpaceship = spaceships[spaceshipCount];
            currentSpaceship.SetActive(true);
        }
    }

    public void PrevSpaceship()
    {
        //LOOP SELECTION
        if (spaceshipCount - 1 < 0)
        {
            currentSpaceship.SetActive(false);
            spaceshipCount = spaceships.Count - 1;
            currentSpaceship = spaceships[spaceshipCount];
            currentSpaceship.SetActive(true);
        }

        else
        {
            currentSpaceship.SetActive(false);
            spaceshipCount--;
            currentSpaceship = spaceships[spaceshipCount];
            currentSpaceship.SetActive(true);
        }
    }

    public void Upgrade()
    {
        selectionInterface.SetActive(false);
        upgradeInterface.SetActive(true);

        cameraTarget = garage.transform;
    }

    public void Select()
    {
        HideAllUpgrades();
        upgradeInterface.SetActive(false);
        selectionInterface.SetActive(true);

        cameraTarget = currentSpaceship.transform;
    }

    public void Race()
    {
        //GameObject s = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Spaceships/" + currentSpaceship.name + ".prefab", typeof(GameObject));
        GameObject s = Resources.Load<GameObject>("Prefabs/Spaceships/" + currentSpaceship.name);
        GameManager.Instance.LoadRace(s);
    }

    public void HideAllUpgrades()
    {
        //ITERATE SPACESHIPS
        for (int i = 0; i < spaceships.Count; i++)
        {
            //ITERATE COMPONENT CATEGORIES (ENGINE, CHASSIS...)
            for (int j = 0; j < 5; j++)
            {
                //ITERATE FOR EACH UPGRADE AVAILABLE
                for (int k = 0; k < spaceships[i].GetComponent<SpaceshipStructure>().upgradesAtlas.engineUpgrades.Length; k++)
                {                 
                    upgradeButtons[i, j, k].SetActive(false);
                }
            }
        }
    }

    public void ShowCategory(string category)
    {
        HideAllUpgrades();

        switch (category)
        {
            case "Engine":

                //ITERATE FOR EACH UPGRADE AVAILABLE
                for (int k = 0; k < currentSpaceship.GetComponent<SpaceshipStructure>().upgradesAtlas.engineUpgrades.Length; k++)
                {                           
                    upgradeButtons[spaceshipCount, 0, k].SetActive(true);
                }
             
                break;

            case "Chassis":

                //ITERATE FOR EACH UPGRADE AVAILABLE
                for (int k = 0; k < currentSpaceship.GetComponent<SpaceshipStructure>().upgradesAtlas.chassisUpgrades.Length; k++)
                {
                    upgradeButtons[spaceshipCount, 1, k].SetActive(true);
                }

                break;

            case "Blaster":

                //ITERATE FOR EACH UPGRADE AVAILABLE
                for (int k = 0; k < currentSpaceship.GetComponent<SpaceshipStructure>().upgradesAtlas.blastersUpgrades.Length; k++)
                {
                    upgradeButtons[spaceshipCount, 3, k].SetActive(true);
                }

                break;

            case "Jet":

                //ITERATE FOR EACH UPGRADE AVAILABLE
                for (int k = 0; k < currentSpaceship.GetComponent<SpaceshipStructure>().upgradesAtlas.jetsUpgrades.Length; k++)
                {
                    upgradeButtons[spaceshipCount, 2, k].SetActive(true);
                }

                break;

            case "Tank":

                //ITERATE FOR EACH UPGRADE AVAILABLE
                for (int k = 0; k < currentSpaceship.GetComponent<SpaceshipStructure>().upgradesAtlas.tanksUpgrades.Length; k++)
                {
                    upgradeButtons[spaceshipCount, 4, k].SetActive(true);
                }

                break;
        }
    }

    public void ChangeEngineUpgrade(EngineUpgrade engineUpgrade)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().upgradesProfile.engineUpgrades = engineUpgrade;
    }

    public void ChangeChassisUpgrade(ChassisUpgrade chassisUpgrade)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().upgradesProfile.chassisUpgrades = chassisUpgrade;
    }

    public void ChangeJetUpgrade(JetUpgrade jetUpgrade)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().upgradesProfile.jetsUpgrades = jetUpgrade;
    }

    public void ChangeBlasterUpgrade(BlasterUpgrade blasterUpgrade)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().upgradesProfile.blastersUpgrades = blasterUpgrade;
    }

    public void ChangeTankUpgrade(TankUpgrade tankUpgrade)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().upgradesProfile.tanksUpgrades = tankUpgrade;
    }
}
