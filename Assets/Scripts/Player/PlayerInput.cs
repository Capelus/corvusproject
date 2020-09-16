using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //MOVEMENT

    //SMOOTH PARAMETERS
    float movX = 0;
    float movY = 0;
    float sensitivity = 3f;
    float dead = 0.001f;
    [HideInInspector]public bool inputEnabled;

    private void Start()
    {
        GameManager.Instance.playerInput = this;
        inputEnabled = false;
    }

    public Vector2 rawMovement
    {     
        get
        {
            Vector2 i = Vector2.zero;
            i.x = Input.GetAxis("Horizontal");
            i.y = Input.GetAxis("Vertical");
            
            return i;
        }
    }

    public Vector2 smoothedMovement
    {      
        get
            {
            float inputX = Input.GetAxis("Horizontal");
            float inputY = Input.GetAxis("Vertical");

            movX = Mathf.MoveTowards(movX, inputX, sensitivity * Time.deltaTime);
            movX = (Mathf.Abs(movX) < dead) ? 0f : movX;

            movY = Mathf.MoveTowards(movY, inputY, sensitivity * Time.deltaTime);
            movY = (Mathf.Abs(movY) < dead) ? 0f : movY;


            return new Vector2(movX, movY);
            }
        
    }

    //ACCELERATION
    public bool accelerate
    {
        get 
        {
            if (Input.GetAxis("Accelerate") > 0) return true;
            return false;
        }
    }

    public float accelerationValue
    {
        get
        {
            return Input.GetAxis("Accelerate");
        }
    }

    //BRAKE
    public bool brake
    {
        get
        {
            if (Input.GetAxis("Accelerate") < 0) return true;
            return false;
        }
    }

    //BARREL ROLL
    public bool roll
    {
        get { return Input.GetButton("Roll"); }
    }

    //NITRO
    public bool nitro
    {
        get { return Input.GetButton("Nitro"); }
    }

    //BLASTER
    public bool blaster
    {
        get { return Input.GetButton("Fire"); }
    }

    //-----------------------------------------DEBUG
    public bool rechargeEnergy
    {
        get { return Input.GetButton("ReloadEnergy"); }
    }
}
