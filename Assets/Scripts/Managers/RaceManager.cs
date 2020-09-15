using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public float countDown = 0;
    public float raceTimer;
    [HideInInspector] public bool raceStarted;
    public float milliseconds, seconds, minutes;
    public string convertedTime;
    public int lapCount;
    public float bestLap;

    [System.Serializable]
    public class Lap
    {
        public float rawTime;
        public string lapConvertedTime;
    }
    public Lap lapLog;

    
    private void Start()
    {
        Instance = this;
        countDown = 3.50f;
        raceStarted = false;
        raceTimer = 0.0f;
        bestLap = 9999999.0f;
        lapCount = 0;
    }
    void Update()
    {
        //COUNTDOWN------------------------------------
        if (countDown > 0) countDown -= Time.deltaTime;
        if (countDown < 1)
        {
            raceStarted = true;
            lapLog.rawTime += Time.deltaTime;
            
        }
        if(raceStarted) raceTimer += Time.deltaTime;
        convertedTime = FormatTime(raceTimer);
    }
    
    string FormatTime(float totalRaceTime)
    {

       
        int minutes  = (int)totalRaceTime / 60;
        int seconds = (int)totalRaceTime % 60;
        float milliseconds = totalRaceTime * 1000;
        milliseconds %= 1000;
        string convertToString = String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        return convertToString;
    }

    public void LapChecker()
    {
        lapLog.lapConvertedTime = FormatTime(lapLog.rawTime);
        
        UIManager.Instance.UpdateTimeChart(lapLog.lapConvertedTime);
        
        lapCount++;
        lapLog.rawTime = 0;
        raceTimer = 0;
        Debug.Log(lapLog.lapConvertedTime);
    }
}
