using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    //-------------------------------------- REFERENCES
    PlayerInput playerInput;
    CameraBehaviour cam;
    //-------------------------------------------------

    //HEALTH
    public float health = 100;

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
    [HideInInspector] public float l_maxSpeed, l_acceleration, currentSpeed, horizontalMove, verticalMove;
    [HideInInspector] public Vector3 forwardDirection = Vector3.left;
    [HideInInspector] public float distanceTravelled = 0;

    [HideInInspector] public bool canMove;
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
    Animator animator;
    //---------------------------------------------------

    private void Awake()
    {
        GameManager.Instance.player = this;
    }

    private void Start()
    {
        //GET REFERENCES
        playerInput = GetComponent<PlayerInput>();
        cam = Camera.main.GetComponent<CameraBehaviour>();
        animator = GetComponent<Animator>();

        //INITIALIZE LOCAL VARIABLES
        l_cadence = blasterParameters.cadence;
        l_maxSpeed = movementParameters.maxSpeed;
        l_energy = energyParameters.initialEnergy;

        //SET INITIAL POSITION
        transform.position = TrackManager.Instance.GetPositionAtDistance(0);// + transform.up * -3;
        canMove = true;
    }

    void Update()
    {

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
            cam.ChangeState(CameraState.moving);
        }
        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------- BARREL ROLL
        if (playerInput.roll)
        {
            //LEFT
            if (playerInput.rawMovement.x < 0)
                animator.SetBool("BarrelRoll_Left", true);

            //RIGHT
            else if (playerInput.rawMovement.x > 0)
                animator.SetBool("BarrelRoll_Right", true);

            //STRAIGHT
            else
            {
                if (Random.value < 0.5f) animator.SetBool("BarrelRoll_Left", true);
                else animator.SetBool("BarrelRoll_Right", true);
            }
        }

        else
        {
            animator.SetBool("BarrelRoll_Left", false);
            animator.SetBool("BarrelRoll_Right", false);
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

                s.tag = "Player";
                s.GetComponent<ProjectileParameters>().initialSpeed = currentSpeed;
                l_cadence = blasterParameters.cadence;
                shotSwitch = !shotSwitch;
                l_energy -= blasterParameters.energyCost;
            }
        }

        //----------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------- MOVEMENT

        //GET FORWARD VECTOR
        forwardDirection = TrackManager.Instance.GetDirectionAtDistance(distanceTravelled);
        transform.forward = forwardDirection.normalized;

        //MOVE
        if (canMove && playerInput.inputEnabled)
            Move();
        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------- OTHER     
        //CLAMP ENERGY
        l_energy = Mathf.Clamp(l_energy, 0, energyParameters.maxEnergy);

        //CHECK HEALTH
        if (health <= 0)
            Explode();
        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------------- DEBUG
        //RECHARGE ENERGY
        if (playerInput.rechargeEnergy)
            RechargeEnergy(1);
        //----------------------------------------------------------------------------------------------------
    }

    void Move()
    {
        //ACCELERATION
        if (playerInput.accelerate)
        {
            while (Mathf.Abs(currentSpeed - l_maxSpeed) > Mathf.Epsilon)
            {
                if (currentSpeed < l_maxSpeed) currentSpeed += l_acceleration * Time.deltaTime;
                else currentSpeed -= l_acceleration * Time.deltaTime;
                break;
            }
        }

        else
        {
            //DECELERATE
            currentSpeed -= l_acceleration * Time.deltaTime;

            if (currentSpeed < 10)
                currentSpeed = 10;

            //CAMERA EFFECT
            cam.ChangeState(CameraState.idle);
        }

        //INCREMENT DISTANCE TRAVELLED
        distanceTravelled += currentSpeed * Time.unscaledDeltaTime;

        //CALCULATE 2D MOVEMENT
        horizontalMove += playerInput.rawMovement.x * movementParameters.handlingSpeed * Time.deltaTime;
        verticalMove += playerInput.rawMovement.y * movementParameters.handlingSpeed * Time.deltaTime;

        //CLAMP ON LIMITS
        horizontalMove = Mathf.Clamp(horizontalMove, -movementParameters.maxWidth, movementParameters.maxWidth);
        verticalMove = Mathf.Clamp(verticalMove, -movementParameters.maxHeight, movementParameters.maxHeight);

        //CALCULATE FINAL MOVEMENT
        Vector3 movementDirection = TrackManager.Instance.GetPositionAtDistance(distanceTravelled);
        movementDirection += transform.right * horizontalMove;
        movementDirection += transform.up * verticalMove;

        //MOVE
        transform.position = movementDirection;

        //ROTATE
        transform.rotation = TrackManager.Instance.GetRotationAtDistance(distanceTravelled);

        //TILT
        animator.SetFloat("Tilt X", playerInput.smoothedMovement.x);
        animator.SetFloat("Tilt Y", playerInput.smoothedMovement.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            //------------------------------------------ ENERGY RING
            case "EnergyRing":
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Spaceship_BarrellRoll_Left") || animator.GetCurrentAnimatorStateInfo(0).IsName("Spaceship_BarrellRoll_Right"))
                {
                    RechargeEnergy(40);
                    Destroy(other.gameObject);
                }
                break;

            case "PitLaneTrigger":
                if (other.name == "PitLane Trigger Exit")
                {
                    cam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("PitLane"));
                    cam.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("RaceTrack");
                }

                else
                {
                    cam.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("PitLane");
                    cam.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RaceTrack"));
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

    public void RechargeEnergy(float amount)
    {
        l_energy += amount;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public void Explode()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().AddExplosionForce(10, transform.position, 5);
        this.enabled = false;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }
}
