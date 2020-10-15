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
    Camera initialSequenceCamera;

    //LOCAL
    [HideInInspector] public float raceTimer = 0;
    [HideInInspector] public int lapCount = 0;
    [HideInInspector] public bool raceStarted;

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
}
