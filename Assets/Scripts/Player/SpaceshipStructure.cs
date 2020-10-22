using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpaceshipStructure : MonoBehaviour
{
    //BASE PROFILE
    public SpaceshipProfile profile;

    //UPGRADES PROFILE
    public UpgradesAtlas upgradesAtlas;
    public UpgradesProfile upgradesProfile;

    //REFERENCES
    public GameObject[] jets;
    public GameObject[] windTrails;
    public GameObject[] overchargeTrails;
    public GameObject[] sparks;
    public Transform cannon;

    private void Start()
    {
        if(GameManager.Instance.player != null)
        {
            GameManager.Instance.player.jets = jets;
            GameManager.Instance.player.windTrails = windTrails;
            GameManager.Instance.player.sparks = sparks;
            GameManager.Instance.player.overchargeTrails = overchargeTrails;
            GameManager.Instance.player.shotSpawn = cannon;
            GameManager.Instance.player.animator = GetComponentInParent<Animator>();
        }        
    }
}
