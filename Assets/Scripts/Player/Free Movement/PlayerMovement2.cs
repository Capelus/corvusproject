using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerMovement2 : MonoBehaviour
{
    //-------------------------------------- REFERENCES
    PlayerInput playerInput;
    CameraBehaviour2 cam;
    //-------------------------------------------------


    //----------------------------- MOVEMENT PARAMETERS  
    //PUBLIC ON INSPECTOR
    public GameObject spaceship;

    [System.Serializable]
    public class MovementParameters
    {
        public float maxSpeed = 150;
        public float acceleration = 40;
        public float turnSpeed = 15;
        public int tiltAngle = 15;

        public float maxWidth = 5;
        public float maxHeight = 5;
    }
    public MovementParameters movementParameters;

    //LOCAL
    [HideInInspector] public float l_maxSpeed, l_acceleration, currentSpeed, horizontalMove, verticalMove;
    [HideInInspector] public Vector3 forwardDirection = Vector3.left;
    [HideInInspector] public float distanceTravelled = 0;

    bool canMove = true;
    //-------------------------------------------------


    //------------------------------------------ ENERGY
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class EnergyParameters
    {
        public float maxEnergy = 300;
        public float initialEnergy = 0;
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

        public GameObject[] jets;

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
    Rigidbody rb;
    public Transform cameraPivot;
    //---------------------------------------------------

    private void Start()
    {
        //GET REFERENCES
        playerInput = GetComponent<PlayerInput>();
        cam = Camera.main.GetComponent<CameraBehaviour2>();

        //INITIALIZE LOCAL VARIABLES
        l_cadence = blasterParameters.cadence;
        l_maxSpeed = movementParameters.maxSpeed;
        l_energy = energyParameters.initialEnergy;
        baseRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        GetComponent<MeshCollider>().sharedMesh = spaceship.GetComponent<MeshFilter>().mesh;

        //SET INITIAL POSITION
        //transform.position = TrackManager.Instance.GetPositionAtDistance(0);
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

            //CAMERA EFFECT
            cam.ChangeState(CameraState.moving);
        }

        else
        {
            //DECELERATE
            currentSpeed -= l_acceleration * Time.deltaTime;

            if (currentSpeed <= 1)
                currentSpeed = 1;

            //CAMERA EFFECT
            cam.ChangeState(CameraState.idle);
        }
        //----------------------------------------------------------------------------------------------------   


        //---------------------------------------------------------------------------------------------- NITRO
        if (playerInput.accelerate && playerInput.nitro && l_energy > 0)
        {
            switch (l_energy)
            {
                //BLUE
                case float e when (l_energy < energyParameters.maxEnergy / 3):
                    //BOOST
                    l_maxSpeed = movementParameters.maxSpeed * nitroParameters.blueNitroMultiplier;
                    l_acceleration = movementParameters.acceleration * nitroParameters.blueNitroMultiplier;

                    //PARTICLES
                    EffectsManager.Instance.effects.warpSpeed = 0.3f;
                    EffectsManager.Instance.effects.nebulaActive = false;

                    foreach (GameObject j in nitroParameters.jets)
                        j.GetComponent<ParticleSystemRenderer>().material.color = Color.cyan;

                    //CAMERA EFFECT
                    cam.ChangeState(CameraState.low_nitro);
                    break;

                //YELLOW
                case float e when (l_energy > energyParameters.maxEnergy / 3 && l_energy < energyParameters.maxEnergy / 3 * 2):
                    //BOOST
                    l_maxSpeed = movementParameters.maxSpeed * nitroParameters.yellowNitroMultiplier;
                    l_acceleration = movementParameters.acceleration * nitroParameters.yellowNitroMultiplier;

                    //PARTICLES
                    EffectsManager.Instance.effects.warpSpeed = 0.6f;
                    EffectsManager.Instance.effects.nebulaActive = true;
                    EffectsManager.Instance.effects.nebulaDissolve = 1;
                    EffectsManager.Instance.effects.nebulaSpeed = 0.3f;

                    foreach (GameObject j in nitroParameters.jets)
                        j.GetComponent<ParticleSystemRenderer>().material.color = Color.yellow;

                    //CAMERA EFFECT
                    cam.ChangeState(CameraState.mid_nitro);
                    break;

                //RED
                case float e when (l_energy > energyParameters.maxEnergy / 3 * 2):
                    //BOOST
                    l_maxSpeed = movementParameters.maxSpeed * nitroParameters.redNitroMultiplier;
                    l_acceleration = movementParameters.acceleration * nitroParameters.redNitroMultiplier;

                    //PARTICLES
                    EffectsManager.Instance.effects.warpSpeed = 1f;
                    EffectsManager.Instance.effects.nebulaActive = true;
                    EffectsManager.Instance.effects.nebulaDissolve = 0.3f;
                    EffectsManager.Instance.effects.nebulaSpeed = 1;

                    foreach (GameObject j in nitroParameters.jets)
                        j.GetComponent<ParticleSystemRenderer>().material.color = Color.red;

                    //CAMERA EFFECT
                    cam.ChangeState(CameraState.high_nitro);
                    break;
            }

            //ENERGY DRAIN
            l_energy -= nitroParameters.energyCost * Time.deltaTime;
        }

        else
        {
            //UN-BOOST
            l_maxSpeed = movementParameters.maxSpeed;
            l_acceleration = movementParameters.acceleration;

            //RESTORE PARTICLES EFFECT
            EffectsManager.Instance.effects.warpSpeed = 0;
            EffectsManager.Instance.effects.nebulaActive = false;

            foreach (GameObject j in nitroParameters.jets)
                j.GetComponent<ParticleSystemRenderer>().material.color = Color.white;

            //RESTORE CAMERA EFFECT
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 75, 0.2f);
        }
        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------- BARREL ROLL
        if (playerInput.roll)
        {
            if (!GetComponent<Animation>().isPlaying)
            {
                if (playerInput.rawMovement.x < 0)
                    GetComponent<Animation>().Play("anim_BarrelRoll_Left");

                else if (playerInput.rawMovement.x > 0)
                    GetComponent<Animation>().Play("anim_BarrelRoll_Right");

                else
                {
                    if (Random.value < 0.5f)
                        GetComponent<Animation>().Play("anim_BarrelRoll_Left");

                    else GetComponent<Animation>().Play("anim_BarrelRoll_Right");
                }
            }
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

                s.GetComponent<BasicShot>().speed += currentSpeed;
                l_cadence = blasterParameters.cadence;
                shotSwitch = !shotSwitch;
                l_energy -= blasterParameters.energyCost;
            }
        }
        //----------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------- MOVEMENT
        if (canMove)
        {
            Rotate();

            ////HORIZONTAL TILTS
            //baseRotation = transform.rotation;

            //if (playerInput.movement.x < 0) //LEFT
            //    transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(0, 0, movementParameters.tiltAngle), 1f * Time.deltaTime);

            //else if (playerInput.movement.x > 0) //RIGHT
            //    transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(0, 0, -movementParameters.tiltAngle), 1f * Time.deltaTime);

            //else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 1f * Time.deltaTime);

            ////VERTICAL TILTS
            //if (playerInput.movement.y < 0) //DOWN
            //    transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(-movementParameters.tiltAngle, 0, 0), 1f * Time.deltaTime);

            //else if (playerInput.movement.y > 0) //UP
            //    transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation * Quaternion.Euler(movementParameters.tiltAngle, 0, 0), 1f * Time.deltaTime);

            //else transform.rotation = Quaternion.Slerp(transform.rotation, baseRotation, 1f * Time.deltaTime);
        }

        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------- OTHER     
        //CLAMP ENERGY
        l_energy = Mathf.Clamp(l_energy, 0, energyParameters.maxEnergy);
        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------- DEBUG
        //RECHARGE ENERGY
        if (playerInput.rechargeEnergy)
            RechargeEnergy(1);
        //----------------------------------------------------------------------------------------------------
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Move();
        }
    }

    void Rotate()
    {
        float yaw = 0;
        if (Mathf.Abs(playerInput.rawMovement.x) > 0.1f)
            yaw = movementParameters.turnSpeed * playerInput.rawMovement.x * Time.deltaTime;

        float pitch = 0;
        if (Mathf.Abs(playerInput.rawMovement.y) > 0.1f)
            pitch = movementParameters.turnSpeed * playerInput.rawMovement.y * Time.deltaTime;

        //ROTATE
        transform.Rotate(pitch, 0, 0);
        transform.Rotate(0, yaw, 0, relativeTo:Space.World);
    }

    void Move()
    {
        //MOVE
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            //------------------------------------------ ENERGY RING
            case "EnergyRing":
                if (GetComponent<Animation>().isPlaying)
                {
                    RechargeEnergy(40);
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

    public void RechargeEnergy(float quantity)
    {
        l_energy += quantity;
    }

    public void Damage(float quantity)
    {

    }

    public void Explode()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().AddExplosionForce(10, transform.position, 5);
        this.enabled = false;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }
}
