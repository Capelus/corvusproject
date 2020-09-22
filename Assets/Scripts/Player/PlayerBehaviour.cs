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
    [HideInInspector] public Animator animator;
    //-------------------------------------------------

    //SPACESHIP PARAMETERS
    public Spaceship playerSpecs;
    ///

    //----------------------------- MOVEMENT PARAMETERS  
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class MovementParameters
    {
        [Header("Velocity")] //--------------------------------

            [Tooltip("The maximum velocity of the spaceship.")]
                public float maxSpeed = 120;

        [Header("Acceleration")] //----------------------------

        [Tooltip("The maximum acceleration of the spaceship.")]
                public float maxAcceleration = 40;

            [Tooltip("The curve defining the spaceship's acceleration. (Over velocity)")]
                public AnimationCurve accelerationCurve;

            [Tooltip("The curve defining the spaceship's deceleration. (Over velocity)")]
                public AnimationCurve brakeCurve;

        [Header("Handling")] //--------------------------------

            [Tooltip("The maximum velocity the spaceship can move horizontally and vertically")]
                public float maxHandlingSpeed = 15;

            [Tooltip("The curve defining the spaceship's handling. (Over velocity)")]
                public AnimationCurve handlingCurve;

        [Header("Screen Limits")] //---------------------------

        [Tooltip("The width the spaceship can move along.")]
            public float maxWidth = 5;

        [Tooltip("The height the spaceship can move along.")]
            public float maxHeight = 5;
    }

    [Header("MOVEMENT")]
    public MovementParameters movementParameters;

    //LOCAL
    [HideInInspector] public float l_maxSpeed, l_acceleration, currentSpeed, horizontalMove, verticalMove, distanceTravelled = 0;
    public float initialDistance;
    [HideInInspector] public Vector3 forwardDirection = Vector3.left;


    float stunTime = 0;
    //-------------------------------------------------


    //------------------------------------------ ENERGY
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class EnergyParameters
    {
        public float maxEnergy = 300;
        public float initialEnergy = 0;
    }

    [Header("ENERGY")]
    public EnergyParameters energyParameters;

    //LOCAL
    [HideInInspector] public float l_energy;
    //-------------------------------------------------


    //------------------------------------------- NITRO
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class JetParameters
    {
        //NORMAL BOOST
        public float boostAcceleration = 2;
        public float boostConsumption = 2;
        
        //SUPERBOOST
        public float superBoostAcceleration = 2;
        public float superBoostConsumption = 2;

        public CameraState cameraState = CameraState.idle;
    }

    [Header("JETS")]
    public JetParameters jetParameters;

    //LOCAL
    bool nitroInputPhase, nitroInputReleased;
    float nitroInputInitialEnergy;
    float nitroInputTime = 0.4f;
    float l_nitroInputTime, superBoostTime;
    [HideInInspector] public bool boosted, superboosted;

    public GameObject[] jets;
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

    [Header("BLASTERS")]
    public BlasterParameters blasterParameters;

    //LOCAL
    [HideInInspector] public float l_cadence;
    [HideInInspector] public bool shotSwitch = false;
    //---------------------------------------------------

    //---------------------------------------- LAPCHECKER
    bool endedLap;

    private void Awake()
    {
        GameManager.Instance.player = this;
    }

    private void Start()
    {
        //GET REFERENCES
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        cam = GameManager.Instance.playerCamera;

        //GET PARAMETERS FROM MENU IF PLAYERSPECS ON GAME MANAGER IS NOT NULL
        if (GameManager.Instance.playerSpecs != null)
            playerSpecs = GameManager.Instance.playerSpecs;

        //INITIALIZE SPACESHIP PARAMETERS
        if (playerSpecs != null)
        {
            //MOVEMENT
            movementParameters.maxSpeed = playerSpecs.maxSpeedValue;
            movementParameters.maxAcceleration = playerSpecs.maxAccelerationvalue;
            movementParameters.accelerationCurve = playerSpecs.accelerationCurve;
            movementParameters.brakeCurve = playerSpecs.brakeCurve;
            movementParameters.maxHandlingSpeed = playerSpecs.handlingValue;
            movementParameters.handlingCurve = playerSpecs.handlingCurve;

            //BLASTERS
            blasterParameters.shot = playerSpecs.projectile;
            blasterParameters.cadence = playerSpecs.cadenceValue;
        }

        //INITIALIZE LOCAL VARIABLES
        l_cadence = blasterParameters.cadence;
        l_maxSpeed = movementParameters.maxSpeed;
        l_energy = energyParameters.initialEnergy;
        l_nitroInputTime = nitroInputTime;

        //SET INITIAL POSITION
        transform.position = TrackManager.Instance.GetPositionAtDistance(initialDistance);// + transform.up * -3;
        distanceTravelled = initialDistance;

        //GET FORWARD VECTOR
        forwardDirection = TrackManager.Instance.GetDirectionAtDistance(distanceTravelled);
        transform.forward = forwardDirection.normalized;
    }

    void Update()
    {
        //MOVEMENT
        if (playerInput.inputEnabled) UpdateMove();
        
        //NITRO 
        UpdateNitro();

        //BARREL ROLL
        UpdateRoll();

        //BLASTERS
        UpdateBlasters(); 
        
        //CLAMP ENERGY
        l_energy = Mathf.Clamp(l_energy, 0, energyParameters.maxEnergy);

        //--------------- DEBUG
            //RECHARGE ENERGY
            if (playerInput.rechargeEnergy)
                RechargeEnergy(30 * Time.deltaTime);
    }

    private void LateUpdate()
    {
        //RESTORE ANIMATOR BOOLS
        animator.SetBool("Impact", false);
    } 

    void UpdateMove()
    {
        //FORWARD MOVEMENT
        stunTime -= Time.unscaledDeltaTime;
        if (stunTime <= 0)
        {
            //BRAKE
            if (playerInput.brake)
            {
                //CALCULATE BRAKE VALUE ON CURVE
                l_acceleration = (movementParameters.brakeCurve.Evaluate(currentSpeed / l_maxSpeed) * movementParameters.maxAcceleration * 4);
               
                //APPLY THROTTLE
                l_acceleration *= playerInput.throttle;
                
                //BRAKE
                currentSpeed += l_acceleration * Time.deltaTime;

                //CHANGE CAMERA
                if (cam.cameraState != CameraState.braking)
                    cam.ChangeState(CameraState.braking);

                //SET ANIMATOR
                animator.SetBool("Brake", true);
            }

            //ACCELERATE
            else if (playerInput.accelerate)
            {
                //GET ACCELERATION VALUE FROM CURVE
                l_acceleration = (movementParameters.accelerationCurve.Evaluate(currentSpeed / l_maxSpeed) * movementParameters.maxAcceleration);

                //APPLY THROTTLE
                l_acceleration *= playerInput.throttle;

                //INCREASE SPEED
                while (Mathf.Abs(currentSpeed - l_maxSpeed) > Mathf.Epsilon)
                {
                    currentSpeed += l_acceleration * Time.deltaTime;
                    break;
                }

                //REDUCE SPEED WHEN OVERLIMIT & NOT BOOSTED
                if(currentSpeed > l_maxSpeed + (l_maxSpeed * 0.1f) && !boosted && !superboosted)
                {
                    currentSpeed -= movementParameters.maxAcceleration * Time.deltaTime;
                }

                //CHANGE CAMERA
                if (cam.cameraState != CameraState.moving)
                    cam.ChangeState(CameraState.moving);

                //SET ANIMATOR
                animator.SetBool("Brake", false);
            }

            //IDLE
            else
            {
                //DECELERATE
                currentSpeed -= movementParameters.maxAcceleration * Time.deltaTime;

                //CAMERA EFFECT
                if (cam.cameraState != CameraState.idle)
                    cam.ChangeState(CameraState.idle);

                //SET ANIMATOR
                animator.SetBool("Brake", false);
            }
        }

        //CLAMP SPEED
        currentSpeed = Mathf.Clamp(currentSpeed, 5, 500);

        //INCREMENT DISTANCE TRAVELLED
        distanceTravelled += currentSpeed * Time.unscaledDeltaTime;

        //CALCULATE BRAKE VALUE ON CURVE
        l_acceleration = (movementParameters.brakeCurve.Evaluate(currentSpeed / l_maxSpeed) * movementParameters.maxAcceleration * 4);

        //APPLY THROTTLE
        l_acceleration *= playerInput.throttle;

        //CALCULATE 2D MOVEMENT VALUE WITH HANDLING CURVE
        float handling = movementParameters.maxHandlingSpeed * movementParameters.handlingCurve.Evaluate(currentSpeed / l_maxSpeed);
        horizontalMove += playerInput.rawMovement.x * handling * Time.deltaTime;
        verticalMove += playerInput.rawMovement.y * handling * Time.deltaTime;

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
        Quaternion targetRotation = TrackManager.Instance.GetRotationAtDistance(distanceTravelled);
        var step = 150 * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);

        //TILT
        animator.SetFloat("Tilt X", playerInput.smoothedMovement.x);
        animator.SetFloat("Tilt Y", playerInput.smoothedMovement.y);
    }

    void UpdateNitro()
    {
        //DETECT PLAYER'S FIRST INPUT
        if (playerInput.nitroPress && !nitroInputPhase)
        {
            nitroInputPhase = true;
            nitroInputReleased = false;
            nitroInputInitialEnergy = l_energy;
        }

        //INPUT PHASE
        if (nitroInputPhase)
        {
            //DID THE PLAYER RELEASE THE INPUT?
            if (playerInput.nitroRelease)
                nitroInputReleased = true;

            //INPUT TIMER
            l_nitroInputTime -= Time.unscaledDeltaTime;
            if (l_nitroInputTime > 0)
            {
                //IF THIS IS NOT THE FIRST FRAME INPUT...
                if (nitroInputReleased)
                {
                    //DETECT SECOND INPUT FOR SUPER BOOST IF NITRO CHARGE IS FULL
                    if (playerInput.nitroPress && nitroInputInitialEnergy == energyParameters.maxEnergy)
                    {
                        //SUPERBOOST
                        Debug.Log("SUPERBOOST");
                        OneShotBoost(l_energy / jetParameters.superBoostConsumption, jetParameters.superBoostAcceleration, CameraState.superboost);
                        l_energy = 0;
                    }
                }
            }

            //IF INPUT TIME ENDS, END INPUT PHASE
            else
            {
                nitroInputPhase = false;
                l_nitroInputTime = nitroInputTime;
            }
        }

        //IS PLAYER HOLDING?
        if (playerInput.nitroHold)
        {
            //IF THERE IS ENERGY...
            if (l_energy > 0)
            {
                //BOOST
                Boost(jetParameters.boostAcceleration, CameraState.boost);
                l_energy -= jetParameters.boostConsumption * Time.deltaTime;
                boosted = true;
            }

            else boosted = false;
        }

        else boosted = false;
    }

    void UpdateRoll()
    {
        if (playerInput.roll)
        {
            //SET ANIMATOR
            animator.SetBool("Brake", false);

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
    }

    void UpdateBlasters()
    {
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
    }

    public void Boost(float accelerationBoost, CameraState camState)
    {       
        currentSpeed += accelerationBoost * Time.deltaTime;
        if (cam.cameraState != camState)
            cam.ChangeState(camState);
    }

    public void OneShotBoost(float duration, float accelerationBoost, CameraState camState)
    {
        StartCoroutine(BoostCoroutine(duration, accelerationBoost, camState));
    }

    IEnumerator BoostCoroutine(float duration, float accelerationBoost, CameraState camState)
    {
        superboosted = true;
        while (duration > 0)
        {
            duration -= Time.deltaTime;

            currentSpeed += accelerationBoost * Time.deltaTime;

            if (cam.cameraState != camState)
                cam.ChangeState(camState);

            yield return null;
        }
        superboosted = false;
        cam.cameraState = CameraState.moving;
        yield break;
    }

    public void RechargeEnergy(float amount)
    {
        l_energy += amount;
    }

    public void TakeDamage(float amount)
    {
        //health -= amount;
    }

    public void Knockback(float amount)
    {
        currentSpeed -= amount;
        stunTime = 0.5f;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }

    public void Explode()
    {
        this.enabled = false;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            //------------------------------------------ ENERGY RING
            case "Nitro":
                RechargeEnergy(30);
                SoundManager.Instance.PlaySound("NitroCluster");
                Destroy(other.gameObject);         
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
                Knockback(30);
                animator.SetBool("Impact", true);
                break;

            case "HardObstacle":
                Knockback(currentSpeed + 20);
                animator.SetBool("Impact", true);
                break;

            case "Enemy":
                Knockback(30);
                animator.SetBool("Impact", true);
                break;

            case "Finish":
                Debug.Log("Lap");
                if (!endedLap)
                {
                RaceManager.Instance.LapChecker();
                }
                endedLap = true;
                break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        switch (collision.transform.tag)
        {
            case "Finish":
                endedLap = false;
                break;
        }

    }
}
