using System;
using System.Collections;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    //SINGLETON
    public static RaceManager Instance;

    //PUBLIC
    public bool initialSequence, warmUpSequence;
    public float countDownTime = 3;
    public GameObject AIRacer1, AIRacer2, AIRacer3, AIRacer4, AIRacer5;
    Camera initialSequenceCamera;

    //LOCAL
    [HideInInspector] public float raceTimer = 0;
    [HideInInspector] public int lapCount = 0;
    [HideInInspector] public bool raceStarted;
    int racePosition;

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
        //GET REFERENCES
        initialSequenceCamera = GameObject.FindGameObjectWithTag("InitialSequenceCamera").GetComponent<Camera>();

        //DISABLE INPUT
        GameManager.Instance.playerInput.inputEnabled = false;

        if (initialSequence)
            LaunchInitialSequence();

        else if (warmUpSequence)
            UIManager.Instance.LaunchWarmUpEvent(countDownTime);       
    }

    void Update()
    {
        if (!initialSequence && !warmUpSequence)
        {
            //ENABLE INPUT
            GameManager.Instance.playerInput.inputEnabled = true;

            //START RACE
            raceStarted = true;

            raceTimer += Time.deltaTime;
            lapLog.rawTime += Time.deltaTime;
        }
        calculatePosition();
    }

    void LaunchInitialSequence()
    {
        StartCoroutine("InitialSequenceCoroutine");
    }

    IEnumerator InitialSequenceCoroutine()
    {
        //SWITCH CAMERAS
        initialSequenceCamera.enabled = true;
        GameManager.Instance.playerCamera.GetComponent<Camera>().enabled = false;

        //START ANIMATION
        initialSequenceCamera.GetComponent<Animation>().Play();

        while (initialSequenceCamera.GetComponent<Animation>().isPlaying)
        {
            yield return null;
        }

        //SWITCH CAMERAS
        initialSequenceCamera.enabled = false;
        GameManager.Instance.playerCamera.GetComponent<Camera>().enabled = true;

        //END
        if (warmUpSequence)
            UIManager.Instance.LaunchWarmUpEvent(countDownTime);

        initialSequence = false;
    }

    public void LapChecker()
    {
        lapLog.lapConvertedTime = UIManager.Instance.FormatTime(lapLog.rawTime);
        
        UIManager.Instance.UpdateTimeChart(lapLog.lapConvertedTime);
        
        lapCount++;
        lapLog.rawTime = 0;
        raceTimer = 0;
        Debug.Log(lapLog.lapConvertedTime);       
    }

    void calculatePosition()
    {
        racePosition = 6;
        //GET AI RACERS DISTANCE TRAVELLED
        float AI1Dis, AI2Dis, AI3Dis, AI4Dis, AI5Dis, PlayerDis;
        AI1Dis = AIRacer1.GetComponent<AIBehaviour>().distanceTravelled;
        AI2Dis = AIRacer2.GetComponent<AIBehaviour>().distanceTravelled;
        AI3Dis = AIRacer3.GetComponent<AIBehaviour>().distanceTravelled;
        AI4Dis = AIRacer4.GetComponent<AIBehaviour>().distanceTravelled;
        AI5Dis = AIRacer5.GetComponent<AIBehaviour>().distanceTravelled;
        PlayerDis = GameManager.Instance.player.distanceTravelled;

        //CALCULATE PLAYER POSITION
        if (PlayerDis > AI1Dis)
        {
            racePosition--;
        }
        if (PlayerDis > AI2Dis)
        {
            racePosition--;
        }
        if (PlayerDis > AI3Dis)
        {
            racePosition--;
        }
        if (PlayerDis > AI4Dis)
        {
            racePosition--;
        }
        if (PlayerDis > AI5Dis)
        {
            racePosition--;
        }

        // CHANGE POSITION TEXT
        UIManager.Instance.updatePosition(racePosition);

    }
}
