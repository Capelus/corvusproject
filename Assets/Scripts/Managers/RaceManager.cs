using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    public float countDown = 0;
    

    private void Start()
    {
        GameManager.Instance.raceManager = this;
        countDown = 3.60f;
    }
    void Update(){
        if(countDown>0) countDown -= Time.deltaTime;
        if (countDown < 0) countDown = 0;

    }
}
