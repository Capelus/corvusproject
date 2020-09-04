﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{



    //REFERENCES
    PlayerBehaviour player;

    //UI ELEMENTS

    [System.Serializable]
    public class UIWarmUp
    {
        //LIST OF UIW ELEMENTS
        public Text countDown;

    }
    public UIWarmUp UIW;

    [System.Serializable]
    public class UIElements
    {
        //LIST OF UI ELEMENTS
        public Text speedometer;
        public Slider energyBarLow;
        public Slider energyBarMid;
        public Slider energyBarHigh;
        public Text raceTimer;
        public Text[] timeChart;
    }
    public UIElements UI;

    private void Start()
    {
        GameManager.Instance.uiManager = this;
        
        player = GameManager.Instance.player;

        UI.timeChart = new Text[8];
        UI.timeChart[0] = GameObject.Find("Lap 1").GetComponent<UnityEngine.UI.Text>();


              UI.energyBarLow.minValue = 0;
        UI.energyBarMid.minValue = (player.energyParameters.maxEnergy / 3);
        UI.energyBarHigh.minValue = (player.energyParameters.maxEnergy / 3) * 2;
        UIW.countDown.text = "GET READY";
        UI.energyBarLow.maxValue = (player.energyParameters.maxEnergy / 3);
        UI.energyBarMid.maxValue = (player.energyParameters.maxEnergy / 3) * 2;
        UI.energyBarHigh.maxValue = player.energyParameters.maxEnergy;
        //UI.timeChart[0].text = "hello";
    }

    private void Update()
    {
        //UPDATE UI
        UI.speedometer.text = Mathf.FloorToInt(player.currentSpeed).ToString();
        UI.energyBarLow.value = player.l_energy;
        UI.energyBarMid.value = player.l_energy;
        UI.energyBarHigh.value = player.l_energy;
        UIW.countDown.text = GameManager.Instance.raceManager.countDown.ToString("f0");
      

        //COUNTDOWN--------------------------------------------
        if (GameManager.Instance.raceManager.countDown <= 1.0f)
        {
            player.GetComponent<PlayerInput>().inputEnabled = true;
            UIW.countDown.text = "GO!";

        }
        if (GameManager.Instance.raceManager.countDown <= 0.0f)
        {
            UIW.countDown.text = "";
        }

        //RACE TIMER--------------------------------------------------------
        UI.raceTimer.text = GameManager.Instance.raceManager.convertedTime;

    }

    public void UpdateTimeChart(string lastLapTime)
    {
        Debug.Log("hello");
        UI.timeChart[GameManager.Instance.raceManager.lapCount].text = lastLapTime;

    }
}
