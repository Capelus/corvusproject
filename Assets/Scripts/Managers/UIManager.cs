using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //REFERENCES
    PlayerMovement player;

    //UI ELEMENTS
    [System.Serializable]
    public class UIElements
    {
        //LIST OF UI ELEMENTS
        public Text speedometer;
        public Text countDown;
        public Slider energyBarLow;
        public Slider energyBarMid;
        public Slider energyBarHigh;
    }
    public UIElements UI;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        UI.energyBarLow.minValue = 0;
        UI.energyBarMid.minValue = (player.energyParameters.maxEnergy / 3);
        UI.energyBarHigh.minValue = (player.energyParameters.maxEnergy / 3) * 2;
        UI.countDown.text = "GET READY";
        UI.energyBarLow.maxValue = (player.energyParameters.maxEnergy / 3);
        UI.energyBarMid.maxValue = (player.energyParameters.maxEnergy / 3) * 2;
        UI.energyBarHigh.maxValue = player.energyParameters.maxEnergy;
    }

    private void Update()
    {
        //UPDATE UI
        UI.speedometer.text = Mathf.FloorToInt(player.currentSpeed).ToString();

        UI.energyBarLow.value = player.l_energy;
        UI.energyBarMid.value = player.l_energy;
        UI.energyBarHigh.value = player.l_energy;
    }
}
