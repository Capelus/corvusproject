using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    //SINGLETON
    public static RaceManager Instance;

    //PUBLIC
    public bool initialSequence, warmUpSequence;
    public float countDownTime = 3;
    public int numberOfLaps;
    public GameObject [] AIRacers;
    Camera initialSequenceCamera;

    //LOCAL
    [HideInInspector] public float lapTimer = 0;
    [HideInInspector] public int lapCount = 0;
    [HideInInspector] public bool raceStarted,raceEnded;
    [HideInInspector] public int actualLap;
    int racePosition;
    float bestLapTime;
    float totalRaceTime;
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
        GameManager.Instance.playerInput.movementEnabled = false;

        if (initialSequence)
            LaunchInitialSequence();

        else if (warmUpSequence)
            UIManager.Instance.LaunchWarmUpEvent(countDownTime);

        //SET LAP VALUES
        actualLap = 1;
        if (numberOfLaps == 0)
            numberOfLaps = 2;
        UIManager.Instance.UI.numberOfLaps.text ="/0"+ numberOfLaps.ToString();
    }

    void Update()
    {
        if (!initialSequence && !warmUpSequence && !raceEnded)
        {
            //ENABLE INPUT & MOVEMENT
            GameManager.Instance.playerInput.movementEnabled = true;
            GameManager.Instance.playerInput.inputEnabled = true;
            //START RACE
            raceStarted = true;
            lapTimer += Time.deltaTime;
            totalRaceTime += Time.deltaTime;
            lapLog.rawTime += Time.deltaTime;

        }
        //CALCULATE ACTUAL POSITION IN RACE
        if(AIRacers!=null)
            CalculatePosition();
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

        //UPDATE TIME CHART
        lapLog.lapConvertedTime = UIManager.Instance.FormatTime(lapLog.rawTime);
        
        UIManager.Instance.UpdateTimeChart(lapLog.lapConvertedTime);
        
        //INCREASE LAP
        lapCount++;
        actualLap++;

        //CHECK IF FASTEST LAP
        if (bestLapTime !=0)
        {
            if (lapTimer < bestLapTime)
            {
                bestLapTime = lapTimer;
                UIManager.Instance.UI.bestLap.text = UIManager.Instance.FormatTime(lapTimer);
            }
        }
        else
        {
            bestLapTime = lapTimer;
            UIManager.Instance.UI.bestLap.gameObject.SetActive(true);
            UIManager.Instance.UI.bestLap.text = UIManager.Instance.FormatTime(lapTimer);

        }
        lapLog.rawTime = 0;
        lapTimer = 0;

        //CHECK IF RACE FINISHED
        if (actualLap > numberOfLaps)
        {
            raceEnded = true;
            RaceEndSequence();
        }
    }

    void CalculatePosition()
    {
        racePosition = 6;
        //GET AI RACERS DISTANCE TRAVELLED
        float AI1Dis, AI2Dis, AI3Dis, AI4Dis, AI5Dis, PlayerDis;

        AI1Dis = AIRacers[0].GetComponent<AIBehaviour>().distanceTravelled;
        AI2Dis = AIRacers[1].GetComponent<AIBehaviour>().distanceTravelled;
        AI3Dis = AIRacers[2].GetComponent<AIBehaviour>().distanceTravelled;
        AI4Dis = AIRacers[3].GetComponent<AIBehaviour>().distanceTravelled;
        AI5Dis = AIRacers[4].GetComponent<AIBehaviour>().distanceTravelled;
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
        UIManager.Instance.UpdatePosition(racePosition);

    }

    void RaceEndSequence()
    {
        GameManager.Instance.playerInput.inputEnabled = false;
        UIManager.Instance.UI.endPanel.SetActive(true);
        UIManager.Instance.DisableRaceUI();
        UIManager.Instance.UI.resetRace.Select();
        switch (racePosition){

            case 1:
                UIManager.Instance.UI.endPanelPositionText.text = "1st";
                UIManager.Instance.UI.endPanelPositionPanel.GetComponent<Image>().color = Color.yellow;
                break;

            case 2:
                UIManager.Instance.UI.endPanelPositionText.text = "2nd";
                UIManager.Instance.UI.endPanelPositionPanel.GetComponent<Image>().color = Color.white;
                break;

            case 3:
                UIManager.Instance.UI.endPanelPositionText.text = "3rd";
                UIManager.Instance.UI.endPanelPositionPanel.GetComponent<Image>().color = new Color(1.0f, 0.64f, 0.0f);
                break;
            case 4:
                UIManager.Instance.UI.endPanelPositionText.text = "4th";
                break;
            case 5:
                UIManager.Instance.UI.endPanelPositionText.text = "5th";
                break;
            case 6:
                UIManager.Instance.UI.endPanelPositionText.text = "6th";
                break;
        }

        UIManager.Instance.UI.endPanelFastestLap.text = UIManager.Instance.FormatTime(bestLapTime);
        UIManager.Instance.UI.endPanelRaceTime.text = UIManager.Instance.FormatTime(totalRaceTime);

    }
}
