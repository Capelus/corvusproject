using System;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    //SINGLETON
    public static RaceManager Instance;

    public bool startSequence;
    public float countDown = 3;
    [HideInInspector] public float raceTimer;
    [HideInInspector] public int lapCount;
    [HideInInspector] public bool raceStarted;
    [HideInInspector] public bool raceReady;

    [System.Serializable]
    public class Lap
    {
        public float rawTime;
        public string lapConvertedTime;
    }
    public Lap lapLog;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (startSequence)
            GameManager.Instance.playerInput.inputEnabled = false;        
    }

    void Update()
    {
        if (raceReady)
        {
            countDown -= Time.deltaTime;
            if (countDown < 0)
            {
                raceStarted = true;
                lapLog.rawTime += Time.deltaTime;

                if (UIManager.Instance.UIW.warmUpQTE.successQTE)
                {
                    GameManager.Instance.player.OneShotBoost(2, 30, false, CameraState.superboost);
                }

                UIManager.Instance.UIW.warmUpQTE.gameObject.SetActive(false);
                UIManager.Instance.UIW.warmUpQTE.successQTE = false;
                UIManager.Instance.UIW.warmUpQTE.enabled = false;
            }

            raceReady = false;
        }
        
        if (raceStarted)
        {
            raceTimer += Time.deltaTime;         
        }
    }

    public void LapChecker()
    {
        UIManager.Instance.UpdateTimeChart(lapLog.lapConvertedTime);
        
        lapCount++;
        lapLog.rawTime = 0;
        raceTimer = 0;
        Debug.Log(lapLog.lapConvertedTime);       
    }
}
