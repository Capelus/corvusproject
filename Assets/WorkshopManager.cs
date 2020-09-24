using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WorkshopManager : MonoBehaviour
{
    //REFERENCES
    public GameObject upgradeInterface;
    public GameObject selectionInterface;
    Camera cam;

    //SPACESHIPS
    List<GameObject> spaceships = new List<GameObject>();
    public Transform garage;

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
        upgradeInterface.SetActive(false);
        selectionInterface.SetActive(true);

        cameraTarget = currentSpaceship.transform;
    }

    public void Race()
    {
        GameObject s = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Spaceships/" + currentSpaceship.name + ".prefab", typeof(GameObject));
        GameManager.Instance.LoadRace(s);
    }

    public void ChangeEngine(EngineProfile engine)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().profile.engineProfile = engine;
    }

    public void ChangeChassis(ChassisProfile chassis)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().profile.chassisProfile = chassis;
    }

    public void ChangeJet(JetProfile jet)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().profile.jetProfile = jet;
    }

    public void ChangeBlaster(BlasterProfile blaster)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().profile.blasterProfile = blaster;
    }

    public void ChangeTank(TankProfile tank)
    {
        currentSpaceship.GetComponent<SpaceshipStructure>().profile.tankProfile = tank;
    }
}
