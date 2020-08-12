using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //MOVEMENT
    public Vector2 movement
    {
        get
        {
            Vector2 i = Vector2.zero;
            i.x = Input.GetAxis("Horizontal");
            i.y = Input.GetAxis("Vertical");
            return i;
        }
    }

    //ACCELERATION
    public bool accelerate
    {
        get { return Input.GetButton("Accelerate"); }
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
        get { return Input.GetKey(KeyCode.Tab); }
    }
}
