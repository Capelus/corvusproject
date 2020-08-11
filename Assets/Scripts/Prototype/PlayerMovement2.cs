﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerMovement2 : MonoBehaviour
{
    //OPTIONS
    public bool allowHorizontalMovement;
    public bool invertVerticalAxis;

    //MOVEMENT PARAMETERS
    [System.Serializable]
    public class Movement
    {
        public float maxSpeed = 60;
        [HideInInspector]
        public float lMaxSpeed = 60;
        public float currentSpeed = 0;
        public float acceleration = 20;
        [HideInInspector]
        public float lAcceleration = 20;
        public float handlingSpeed = 15;
    }
    public Movement movement;

    public float maxEnergy = 100;
    float energy;

    //SHOT REFERENCES AND PARAMETERS
    [System.Serializable]
    public class Blaster
    {
        public Transform shotSpawn1, shotSpawn2;
        public GameObject shot;
        public float cadence = 0.2f;
    }
    public Blaster blaster;

    float lCadence;
    bool lastShotSwitch = false;

    //LIMITS
    [System.Serializable]
    public class ScreenLimits
    {
        public float bottomLimit = 1, topLimit = 5.5f, leftLimit = 11f, rightLimit = 19f;
    }
    public ScreenLimits screenLimits;

    //OTHER
    public Text velocimeter;
    public Slider energyBar;
    public GameObject explosion;
    Quaternion baseRotation;

    private void Start()
    {
        lCadence = blaster.cadence;
        movement.lMaxSpeed = movement.maxSpeed;
        energy = maxEnergy;
        baseRotation = transform.rotation;
    }

    void Update()
    {
        float vMove = Input.GetAxis("Vertical");
        if (invertVerticalAxis)
            vMove = -vMove;

        float hMove = 0;
        if (allowHorizontalMovement)
            hMove = Input.GetAxis("Horizontal");

        //CHECK IZQUIERDA
        if (hMove < 0)
        {
            if (transform.position.z <= screenLimits.leftLimit)
                hMove = 0;

            else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(0, 0, 15), 0.05f);
        }

        //CHECK DERECHA
        else if (hMove > 0)
        {
            if (transform.position.z >= screenLimits.rightLimit)
                hMove = 0;

            else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(0, 0, -15), 0.05f);
        }

        else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 0.02f);

        //CHECK ARRIBA
        if (vMove > 0)
        {
            if (transform.position.y >= screenLimits.topLimit)
                vMove = 0;

            else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(-15, 0, 0), 0.05f);
        }

        //CHECK ABAJO
        else if (vMove < 0)
        {
            if (transform.position.y <= screenLimits.bottomLimit)
                vMove = 0;

            else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(15, 0, 0), 0.05f);
        }

        else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 0.02f);

        //ACCELERATE
        if (Input.GetButton("Accelerate"))
        {
            //NITRO
            if (Input.GetButton("Nitro") && energy > 0)
            {
               
                Debug.Log("NITRO");

                movement.maxSpeed = movement.lMaxSpeed * 2;
                movement.acceleration = movement.lAcceleration * 5;

                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 110, 0.2f);

                energy -= 20 * Time.deltaTime;
            }               

            else
            {
                movement.maxSpeed = 200;
                movement.acceleration = 20;

                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 75, 0.2f);
            }

            while (Mathf.Abs(movement.currentSpeed - movement.maxSpeed) > Mathf.Epsilon)
            {
                if (movement.currentSpeed < movement.maxSpeed) movement.currentSpeed += movement.acceleration * Time.deltaTime;
                else movement.currentSpeed -= movement.acceleration * Time.deltaTime;
                break;
            }
        }

        else movement.currentSpeed = Mathf.Lerp(movement.currentSpeed, 0, 0.05f);  

        //BARREL ROLL
        if (Input.GetButton("Roll"))
            GetComponent<Animation>().Play();

        //SHOOT
        if (Input.GetButton("Fire"))
        {
            lCadence -= Time.deltaTime;
            if (lCadence < 0)
            {
                GameObject s;

                if (lastShotSwitch)
                    s = Instantiate(blaster.shot, blaster.shotSpawn2.position, blaster.shotSpawn2.rotation);

                else
                    s = Instantiate(blaster.shot, blaster.shotSpawn1.position, blaster.shotSpawn1.rotation);

                s.GetComponent<ShotBehaviour>().speed += movement.currentSpeed;
                lCadence = blaster.cadence;
                lastShotSwitch = !lastShotSwitch;
            }
        }

        //APPLY MOVEMENT
        Vector3 movementDirection = Vector3.up * vMove + Vector3.forward * hMove + Vector3.left;
        Move(movementDirection, movement.maxSpeed);

        //OTHER
        energy = Mathf.Clamp(energy, 0, 100);

        //UPDATE UI
        velocimeter.text = Mathf.FloorToInt(movement.currentSpeed).ToString();
        energyBar.value = energy / maxEnergy;           
    }

    void Move(Vector3 dir, float speed)
    {
        transform.position += new Vector3(dir.x * movement.currentSpeed, dir.y * movement.handlingSpeed, dir.z * movement.handlingSpeed) * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EnergyRing"))
        {
            if (GetComponent<Animation>().isPlaying)
            {       
                energy += 40;
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            this.enabled = false;
            Instantiate(explosion, transform.position, transform.rotation);
        }
    }
}
