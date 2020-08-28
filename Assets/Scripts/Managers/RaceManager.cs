using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    public float countDown;
    private void Awake(){

        GameManager.Instance.raceManager = this;
    }

    private void Start()
    {
        countDown = 3.0f;
    }
    void Update(){
        countDown -= Time.deltaTime;
    }
}
