using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UIManager : MonoBehaviour
{
    //SINGLETON
    public static UIManager Instance;

    //UI ELEMENTS
    [System.Serializable]
    public class UIWarmUp
    {
        //LIST OF UIW ELEMENTS
        public Text countDown;
        public WarmBehaviourQTE warmUpQTE;
        public Image GlowEffect;

    }
    public UIWarmUp UIW;

    [System.Serializable]
    public class UIElements
    {
        //LIST OF UI ELEMENTS
        public Text speedometer;
        public Slider energyBarSlider;
        public Image energyBar;
        public Image AButton;
        public Text raceTimer;
        public Text positionNumber;
        public Text positionSufix;
        public GameObject skillcheck;
        public Text actualLap;
        public Text numberOfLaps;
        public Text bestLap;
        public Text[] timeChart;
        public GameObject endPanel; 
    }
    public UIElements UI;

    //ADDITIONAL ELEMENTS
    Vector3 bigGlowingScale, initialGlowingScale;
    bool glowIncreasing = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //DEFINE WARMUP GLOWING SCALE
        initialGlowingScale = UIW.GlowEffect.rectTransform.localScale;
        bigGlowingScale = new Vector3(UIW.GlowEffect.rectTransform.localScale.x * 1.2f, UIW.GlowEffect.rectTransform.localScale.y * 1.2f, 1);

        //INITIALIZE TIME CHART
        UI.timeChart = new Text[8];
        UI.timeChart[0] = GameObject.Find("Lap 1").GetComponent<UnityEngine.UI.Text>();
        UI.timeChart[1] = GameObject.Find("Lap 2").GetComponent<UnityEngine.UI.Text>();
        UI.timeChart[2] = GameObject.Find("Lap 3").GetComponent<UnityEngine.UI.Text>();
        UI.timeChart[3] = GameObject.Find("Lap 4").GetComponent<UnityEngine.UI.Text>();
        UI.timeChart[4] = GameObject.Find("Lap 5").GetComponent<UnityEngine.UI.Text>();
        UI.timeChart[5] = GameObject.Find("Lap 6").GetComponent<UnityEngine.UI.Text>();
        UI.timeChart[6] = GameObject.Find("Lap 7").GetComponent<UnityEngine.UI.Text>();
        UI.timeChart[7] = GameObject.Find("Lap 8").GetComponent<UnityEngine.UI.Text>();

        //INITIALIZE ENERGY BAR
        UI.energyBarSlider.minValue = 0;
        UI.energyBarSlider.maxValue = GameManager.Instance.player.energyParameters.maxEnergy;
    }

    private void Update()
    {
        //UPDATE UI
        UI.speedometer.text = Mathf.FloorToInt(GameManager.Instance.player.currentSpeed).ToString();
        UI.energyBarSlider.value = GameManager.Instance.player.l_energy;

        UI.energyBar.fillAmount = GameManager.Instance.player.l_energy / GameManager.Instance.player.energyParameters.maxEnergy;

        if (GameManager.Instance.player.l_energy > GameManager.Instance.player.energyParameters.maxEnergy - 1)
        {
            //GLOW OUTLINE
            UI.energyBar.GetComponent<Outline>().effectColor = new Color(1, 1, 1, Mathf.Sin(30 * Time.deltaTime));
            UI.AButton.color = new Color(1, 1, 1, 1);
        }

        else
        {
            UI.energyBar.GetComponent<Outline>().effectColor = new Color(1, 1, 1, 0);
            UI.AButton.color = new Color(1, 1, 1, 0);
        }

        //UPDATE RACE TIMER
        UI.raceTimer.text = FormatTime(RaceManager.Instance.lapTimer);
        //UPDATE LAP 
        UI.actualLap.text = "0"+RaceManager.Instance.actualLap.ToString();
    }

    public void LaunchWarmUpEvent(float countDownTime)
    {
        StartCoroutine("WarmUpCoroutine", countDownTime);
    }

    IEnumerator WarmUpCoroutine(float countDownTime)
    {
        //ACTIVATE UI ELEMENTS
        UIW.countDown.enabled = true;
        UIW.warmUpQTE.gameObject.SetActive(true);
        UIW.GlowEffect.gameObject.SetActive(true);

        //COUNTDOWN
        while (countDownTime > 0)
        {
            countDownTime -= Time.deltaTime;
            UIW.countDown.text = Mathf.Clamp(countDownTime, 1, countDownTime).ToString("f0");
            QTEGlowing();
            yield return null;
        }

        //GO!
        UIW.countDown.text = "GO!";

        //ENABLE INPUT
        GameManager.Instance.playerInput.movementEnabled = true;

        //CHECK QUICK TIME EVENT
        if (UIManager.Instance.UIW.warmUpQTE.successQTE)
            GameManager.Instance.player.OneShotBoost(2, 30, false, CameraState.superboost);

        //DISABLE QUICK TIME EVENT
        UIW.warmUpQTE.gameObject.SetActive(false);
        UIW.warmUpQTE.successQTE = false;
        UIW.warmUpQTE.enabled = false;
        UIW.GlowEffect.gameObject.SetActive(false);

        //START RACE
        RaceManager.Instance.warmUpSequence = false;

        //WAIT FOR "GO" TO BE DISPLAYED
        yield return new WaitForSeconds(0.3f);

        //DISABLE COUNTDOWN
        UIW.countDown.enabled = false;
    }

    //RAW TO MIN/SEC/MILISEC
    public string FormatTime(float totalRaceTime)
    {
        int minutes = (int)totalRaceTime / 60;
        int seconds = (int)totalRaceTime % 60;
        float milliseconds = totalRaceTime * 1000;
        milliseconds %= 1000;
        string convertToString = String.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        return convertToString;
    }

    

    public void UpdateTimeChart(string lastLapTime)
    {
        //UI.timeChart[RaceManager.Instance.lapCount].text = lastLapTime;
    }

    void QTEGlowing()
    {
        if(glowIncreasing)
        {
            if (UIW.GlowEffect.rectTransform.localScale.x < bigGlowingScale.x-0.01f)
                UIW.GlowEffect.rectTransform.localScale = Vector3.Lerp(UIW.GlowEffect.rectTransform.localScale, bigGlowingScale, 0.02f);
            
            else glowIncreasing = false;     
        }

        else
        {
            if(UIW.GlowEffect.rectTransform.localScale.x > initialGlowingScale.x+0.01f)
                UIW.GlowEffect.rectTransform.localScale = Vector3.Lerp(UIW.GlowEffect.rectTransform.localScale, initialGlowingScale, 0.02f);
            
            else glowIncreasing = true;     
        }
    }

    public void updatePosition(int newPosition)
    {
        switch (newPosition)
        {
            case 1:

                UI.positionNumber.text = "1";
                UI.positionSufix.text = "st";
                break;
            case 2:
                UI.positionNumber.text = "2";
                UI.positionSufix.text = "nd";
                break;
            case 3:
                UI.positionNumber.text = "3";
                UI.positionSufix.text = "rd";
                break;
            case 4:
                UI.positionNumber.text = "4";
                UI.positionSufix.text = "th";
                break;
            case 5:
                UI.positionNumber.text = "5"; 
                UI.positionSufix.text = "th";
                break;
            case 6:
                UI.positionNumber.text = "6";
                UI.positionSufix.text = "th";
                break;


        }


    }
}
