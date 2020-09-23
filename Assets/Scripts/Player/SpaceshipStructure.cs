using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipStructure : MonoBehaviour
{
    public GameObject[] jets;
    public Transform cannon;

    private void Start()
    {
        GameManager.Instance.player.jets = jets;
        GameManager.Instance.player.shotSpawn = cannon;
        GameManager.Instance.player.animator = GetComponentInParent<Animator>();
    }
}
