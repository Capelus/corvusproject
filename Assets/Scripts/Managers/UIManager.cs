using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    //REFERENCES
    PlayerBehaviour player;

    //UI ELEMENTS

    [System.Serializable]
    public class UIWarmUp
    {
        //LIST OF UIW ELEMENTS
        public Text countDown;
        public WarmBehaviourQTE warmUpQTE;
        public Image RTbutton;
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
        public Text[] timeChart;
        public GameObject skillcheck;
    }
    public UIElements UI;

    private void Start()
    {
        Instance = this;     
        player = GameManager.Instance.player;

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
        UI.energyBarSlider.maxValue = player.energyParameters.maxEnergy;

        UIW.countDown.text = "HOLD";
        UIW.countDown.enabled = false;

    }

    private void Update()
    {
        //UPDATE UI
        UI.speedometer.text = Mathf.FloorToInt(player.currentSpeed).ToString();
        UI.energyBarSlider.value = player.l_energy;

        UI.energyBar.fillAmount = player.l_energy / player.energyParameters.maxEnergy;

        if (player.l_energy > player.energyParameters.maxEnergy - 1)
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

        if (RaceManager.Instance.countDownReady)
        {
            UIW.countDown.text = RaceManager.Instance.countDown.ToString("f0");
        }
      

        //COUNTDOWN--------------------------------------------
        if (RaceManager.Instance.countDown <= 1.0f)
        {
            player.GetComponent<PlayerInput>().inputEnabled = true;
            UIW.countDown.text = "GO!";

        }
        if (RaceManager.Instance.countDown <= 0.0f)
        {
            UIW.countDown.text = "";
        }

        //RACE TIMER--------------------------------------------------------
        UI.raceTimer.text = RaceManager.Instance.convertedTime;
    }

    public void UpdateTimeChart(string lastLapTime)
    {
        UI.timeChart[RaceManager.Instance.lapCount].text = lastLapTime;
    }
}
