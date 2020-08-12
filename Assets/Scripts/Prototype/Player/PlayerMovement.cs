using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //-------------------------------------- REFERENCES
        PlayerInput playerInput;
    //-------------------------------------------------


    //----------------------------- MOVEMENT PARAMETERS  
        //PUBLIC ON INSPECTOR
        [System.Serializable]
        public class MovementParameters
        {
            public float maxSpeed = 150;
            public float acceleration = 40;
            public float handlingSpeed = 15;
            public int tiltAngle = 15;

            public float maxWidth = 5;
            public float maxHeight = 5;
        }
        public MovementParameters movementParameters;

        //LOCAL
        [HideInInspector] public float l_maxSpeed, l_acceleration, currentSpeed;
        [HideInInspector] public Vector3 forwardDirection = Vector3.left;
        bool canMove = true;
    //-------------------------------------------------


    //------------------------------------------ ENERGY
        //PUBLIC ON INSPECTOR
        [System.Serializable]
        public class EnergyParameters
        {
            public float maxEnergy = 100;
        }
        public EnergyParameters energyParameters;

        //LOCAL
        [HideInInspector] public float l_energy;
    //-------------------------------------------------


    //------------------------------------------- NITRO
        //PUBLIC ON INSPECTOR
        [System.Serializable]
        public class NitroParameters
        {
            public float blueNitroMultiplier = 2;
            public float yellowNitroMultiplier = 3;
            public float redNitroMultiplier = 5;

            public float energyCost = 5;
        }
        public NitroParameters nitroParameters;
    //-------------------------------------------------


    //------------------------------------------ BLASTER
        //PUBLIC ON INSPECTOR
        [System.Serializable]
        public class BlasterParameters
        {
            public Transform shotSpawn1, shotSpawn2;
            public GameObject shot;
            public float cadence = 0.2f;

            public float energyCost = 1;
        }
        public BlasterParameters blasterParameters;

        //LOCAL
        [HideInInspector] public float l_cadence;
        [HideInInspector] public bool shotSwitch = false;
    //---------------------------------------------------


    //------------------------------------ EXTRA SETTINGS
        //PUBLIC ON INSPECTOR
        [System.Serializable]
        public class ExtraSettings
        {
            public bool allowHorizontalMovement = true;
            public bool invertVerticalAxis;
        }
        public ExtraSettings extraSettings;
    //---------------------------------------------------


    //--------------------------------------------- OTHER
        Quaternion baseRotation;
    //---------------------------------------------------

    private void Start()
    {
        //GET REFERENCES
        playerInput = GetComponent<PlayerInput>();

        //INITIALIZE LOCAL VARIABLES
        l_cadence = blasterParameters.cadence;
        l_maxSpeed = movementParameters.maxSpeed;
        l_energy = energyParameters.maxEnergy;
        baseRotation = transform.rotation;
    }

    void Update()
    {
        //--------------------------------------------------------------------------------------- ACCELERATION
        if (playerInput.accelerate)
        {
            while (Mathf.Abs(currentSpeed - l_maxSpeed) > Mathf.Epsilon)
            {
                if (currentSpeed < l_maxSpeed) currentSpeed += l_acceleration * Time.deltaTime;
                else currentSpeed -= l_acceleration * Time.deltaTime;
                break;
            }
        }

        else currentSpeed = Mathf.Lerp(currentSpeed, 0, 0.05f);
        //----------------------------------------------------------------------------------------------------   


        //---------------------------------------------------------------------------------------------- NITRO
        if (playerInput.accelerate && playerInput.nitro && l_energy > 0)
        {
            //BOOST
            l_maxSpeed = movementParameters.maxSpeed * nitroParameters.blueNitroMultiplier;
            l_acceleration = movementParameters.acceleration * nitroParameters.blueNitroMultiplier;

            //CAMERA EFFECT
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 110, 0.2f);

            //ENERGY DRAIN
            l_energy -= nitroParameters.energyCost * Time.deltaTime;
        }

        else
        {
            //UN-BOOST
            l_maxSpeed = movementParameters.maxSpeed;
            l_acceleration = movementParameters.acceleration;

            //RESTORE CAMERA EFFECT
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 75, 0.2f);
        }
        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------- BARREL ROLL
        if (playerInput.roll)
        {
            GetComponent<Animation>().Play();
        }
        //----------------------------------------------------------------------------------------------------


        //-------------------------------------------------------------------------------------------- BLASTER
        if (playerInput.blaster && l_energy > 0)
        {
            l_cadence -= Time.deltaTime;
            if (l_cadence < 0)
            {
                GameObject s;

                if (shotSwitch)
                    s = Instantiate(blasterParameters.shot, blasterParameters.shotSpawn2.position, blasterParameters.shotSpawn2.rotation);

                else
                    s = Instantiate(blasterParameters.shot, blasterParameters.shotSpawn1.position, blasterParameters.shotSpawn1.rotation);

                s.GetComponent<ShotBehaviour>().speed += currentSpeed;
                l_cadence = blasterParameters.cadence;
                shotSwitch = !shotSwitch;
                l_energy -= blasterParameters.energyCost;
            }
        }
        //----------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------- MOVEMENT
        if (canMove)
        {
            Move();

            //HORIZONTAL TILTS
            if(playerInput.movement.x < 0) //LEFT
                transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(0, 0, movementParameters.tiltAngle), 0.05f);
            
            else if(playerInput.movement.x > 0) //RIGHT
                transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(0, 0, -movementParameters.tiltAngle), 0.05f);
            
            else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 0.02f);
            
            //VERTICAL TILTS
            if(playerInput.movement.y < 0) //DOWN
                transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(-movementParameters.tiltAngle, 0, 0), 0.05f);

            else if (playerInput.movement.y > 0) //UP
                transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(movementParameters.tiltAngle, 0, 0), 0.05f);

            else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 0.02f);
        }

        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------- OTHER
        l_energy = Mathf.Clamp(l_energy, 0, 100);
        //----------------------------------------------------------------------------------------------------
    }

    void Move()
    {
        Vector3 movementDirection = 
            (transform.right * playerInput.movement.x * movementParameters.handlingSpeed) +  //HORIZONTAL
            (Vector3.up * playerInput.movement.y * movementParameters.handlingSpeed) +       //VERTICAL
            (forwardDirection * currentSpeed);                                               //FORWARD

        transform.position += movementDirection * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            //------------------------------------------ ENERGY RING
            case "EnergyRing":
                if (GetComponent<Animation>().isPlaying)
                {
                    l_energy += 40;
                    Destroy(other.gameObject);
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.transform.tag)
        {
            //--------------------------------------------- OBSTACLE
            case "Obstacle": 
                Explode(); 
                break;
        }
    }

    public void Damage(float dmg)
    {

    }

    public void Explode()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().AddExplosionForce(10, transform.position, 5);
        this.enabled = false;
        GameManager.gm.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }
}
