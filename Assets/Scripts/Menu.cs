using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    public static Menu Instance;
    public Text maxSpeedvalue, accValue, handlingValue;
   
    //BUTTON LIST
    Button TierC, TierB, TierA, TierS;

    //PARAMETERS TEST FOR DEBUGGING
    public float maxSpeed;
    public float acceleration;
    public float handlingSpeed;
    public int tiltAngle;
    public float maxWidth;
    public float maxHeight;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update(){
        
    }


    public void ChangeScene(Button pressedButton)
    {

        switch (pressedButton.name)
        {
            case "TIER C":
                maxSpeed = 100;
                acceleration = 30;
                handlingSpeed = 10;
                break;

            case "TIER B":
                maxSpeed = 130;
                acceleration = 35;
                handlingSpeed = 12;
                break;

            case "TIER A":
                maxSpeed = 150;
                acceleration = 40;
                handlingSpeed = 15;         
                break;

            case "TIER S":
                maxSpeed = 180;
                acceleration = 45;
                handlingSpeed = 18;
                break;

            case "CUSTOM TIER":
                maxSpeed = float.Parse(maxSpeedvalue.text.ToString());
                acceleration = float.Parse(accValue.text.ToString());
                handlingSpeed = float.Parse(handlingValue.text.ToString());               
                break;

            default:
                break;
        }

        SceneManager.LoadScene(1);
    }
}
