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

    [Header("- - - - - - - - - - - - - - - - - - - - - - - - - - - -")]
    public MovementParameters movementParameters;

    //LOCAL
    [HideInInspector] public float l_maxSpeed, l_acceleration, currentSpeed, horizontalMove, verticalMove, distanceTravelled = 0;
    public float initialDistance;
    [HideInInspector] public Vector3 forwardDirection = Vector3.left;
    [HideInInspector] public bool canMove;

    bool boosted = false;
    float stunTime = 0;
    float overSpeed = 0;
    //-------------------------------------------------


    //------------------------------------------ ENERGY
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class EnergyParameters
    {
        public float maxEnergy = 300;
        public float initialEnergy = 0;
    }

    [Header("- - - - - - - - - - - - - - - - - - - - - - - - - - - -")]
    public EnergyParameters energyParameters;

    //LOCAL
    [HideInInspector] public float l_energy;
    //-------------------------------------------------


    //------------------------------------------- NITRO
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class NitroParameters
    {
        public float boostTime = 2;
        public float speedBoost = 2;
        public float accelerationBoost = 2;
        public CameraState cameraState = CameraState.low_nitro;
    }

    [Header("- - - - - - - - - - - - - - - - - - - - - - - - - - - -")]
    public NitroParameters blueNitro;
    public NitroParameters yellowNitro;
    public NitroParameters redNitro;

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

    [Header("- - - - - - - - - - - - - - - - - - - - - - - - - - - -")]
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
        //GET TIER FROM MENU IF PLAYERSPECS ON GAME MANAGER IS NOT NULL
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

        //GET REFERENCES
        playerInput = GetComponent<PlayerInput>();
        cam = Camera.main.GetComponent<CameraBehaviour>();
        animator = GetComponent<Animator>();

        //INITIALIZE LOCAL VARIABLES
        l_cadence = blasterParameters.cadence;
        l_maxSpeed = movementParameters.maxSpeed;
        l_energy = energyParameters.initialEnergy;

        //SET INITIAL POSITION
        transform.position = TrackManager.Instance.GetPositionAtDistance(initialDistance);// + transform.up * -3;
        distanceTravelled = initialDistance;
        canMove = true;
    }

    void Update()
    {
        //---------------------------------------------------------------------------------------------- NITRO
        if (playerInput.accelerate && playerInput.nitro && l_energy > energyParameters.maxEnergy / 3 - 1)
        {
            switch (l_energy)
            {
                //BLUE
                case float e when (l_energy < energyParameters.maxEnergy / 3):

                    //BOOST
                    Boost(blueNitro.boostTime, blueNitro.speedBoost, blueNitro.accelerationBoost, blueNitro.cameraState);

                    //PARTICLES
                    EffectsManager.Instance.effects.warpSpeed = 0.3f;
                    EffectsManager.Instance.effects.nebulaActive = false;

                    foreach (GameObject j in jets)
                        j.GetComponent<ParticleSystemRenderer>().material.color = Color.cyan;

                    break;

                //YELLOW
                case float e when (l_energy > energyParameters.maxEnergy / 3 && l_energy < energyParameters.maxEnergy / 3 * 2 - 1):

                    //BOOST
                    Boost(yellowNitro.boostTime, yellowNitro.speedBoost, yellowNitro.accelerationBoost, yellowNitro.cameraState);

                    //PARTICLES
                    EffectsManager.Instance.effects.warpSpeed = 0.6f;
                    EffectsManager.Instance.effects.nebulaActive = true;
                    EffectsManager.Instance.effects.nebulaDissolve = 1;
                    EffectsManager.Instance.effects.nebulaSpeed = 0.3f;

                    foreach (GameObject j in jets)
                        j.GetComponent<ParticleSystemRenderer>().material.color = Color.yellow;

                    break;

                //RED
                case float e when (l_energy > energyParameters.maxEnergy / 3 * 2):

                    //BOOST
                    Boost(redNitro.boostTime, redNitro.speedBoost, redNitro.accelerationBoost, redNitro.cameraState);

                    //PARTICLES
                    EffectsManager.Instance.effects.warpSpeed = 1f;
                    EffectsManager.Instance.effects.nebulaActive = true;
                    EffectsManager.Instance.effects.nebulaDissolve = 0.3f;
                    EffectsManager.Instance.effects.nebulaSpeed = 1;

                    foreach (GameObject j in jets)
                        j.GetComponent<ParticleSystemRenderer>().material.color = Color.red;

                    break;
            }

            //ENERGY DRAIN
            l_energy -= energyParameters.maxEnergy / 3 - 1;
        }

        else
        {
            if (!boosted && cam.cameraState != CameraState.ring_skillcheck)
            {
                //UNBOOST
                l_maxSpeed = movementParameters.maxSpeed;
                l_acceleration = movementParameters.maxAcceleration;

                //RESTORE PARTICLES EFFECT
                EffectsManager.Instance.effects.warpSpeed = 0;
                EffectsManager.Instance.effects.nebulaActive = false;

                //RESTORE CAMERA EFFECT
                cam.ChangeState(CameraState.moving);

                foreach (GameObject j in jets)
                    j.GetComponent<ParticleSystemRenderer>().material.color = Color.white;
            }
        }
        //----------------------------------------------------------------------------------------------------


        //---------------------------------------------------------------------------------------- BARREL ROLL
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

        //MOVE
        if (canMove && playerInput.inputEnabled)
                Move();

        else
        {
            //GET FORWARD VECTOR
            forwardDirection = TrackManager.Instance.GetDirectionAtDistance(distanceTravelled);
            transform.forward = forwardDirection.normalized;
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

    private void LateUpdate()
    {
        //RESTORE ANIMATOR BOOLS
        animator.SetBool("Impact", false);
    }

    void Move()
    {
        //FORWARD MOVEMENT
        stunTime -= Time.unscaledDeltaTime;
        if (stunTime <= 0)
        {
            //BRAKE
            if (playerInput.brake)
            {
                //CALCULATE BRAKE VALUE ON CURVE
                l_acceleration *= (movementParameters.brakeCurve.Evaluate(currentSpeed / l_maxSpeed + overSpeed) * -playerInput.throttle);

                //BRAKE
                currentSpeed -= l_acceleration * 3 * Time.deltaTime;
                
                //CHANGE CAMERA
                cam.ChangeState(CameraState.braking);

                //SET ANIMATOR
                animator.SetBool("Brake", true);
            }

            //ACCELERATE
            else if (playerInput.accelerate)
            {
                l_acceleration *= (movementParameters.accelerationCurve.Evaluate(currentSpeed / l_maxSpeed + overSpeed) * playerInput.throttle);

                while (Mathf.Abs(currentSpeed - l_maxSpeed + overSpeed) > Mathf.Epsilon)
                {
                    if (currentSpeed < l_maxSpeed - 30) currentSpeed += l_acceleration * Time.deltaTime;

                    //GRADUALLY DECREASE ACCELERATION WHEN REACHING SPEED LIMIT
                    else if (currentSpeed < l_maxSpeed - 10 && currentSpeed > l_maxSpeed - 30) currentSpeed += l_acceleration / 4f * Time.deltaTime;
                    else if (currentSpeed < l_maxSpeed - 5 && currentSpeed > l_maxSpeed - 10) currentSpeed += l_acceleration / 6f * Time.deltaTime;

                    //SLIGHTLY SPEED UP OVER LIMIT
                    else if (currentSpeed < l_maxSpeed + overSpeed)
                    {
                        currentSpeed += 0.3f * Time.deltaTime;
                        overSpeed += 0.3f * Time.deltaTime;
                    }

                    //DECELERATE
                    else
                    {
                        overSpeed = 0;
                        currentSpeed -= l_acceleration / 2 * Time.deltaTime;
                    }

                    break;
                }

                //SET ANIMATOR
                animator.SetBool("Brake", false);
            }

            //IDLE
            else
            {
                //DECELERATE
                currentSpeed -= l_acceleration * Time.deltaTime;
                overSpeed = 0;

                //CAMERA EFFECT
                cam.ChangeState(CameraState.idle);

                //SET ANIMATOR
                animator.SetBool("Brake", false);
            }
        }

        //CLAMP SPEED
        currentSpeed = Mathf.Clamp(currentSpeed, 5, 1000);

        //INCREMENT DISTANCE TRAVELLED
        distanceTravelled += currentSpeed * Time.unscaledDeltaTime;

        //CALCULATE 2D MOVEMENT
        horizontalMove += playerInput.rawMovement.x * movementParameters.maxHandlingSpeed * Time.deltaTime;
        verticalMove += playerInput.rawMovement.y * movementParameters.maxHandlingSpeed * Time.deltaTime;

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

    public void Boost(float time, float speedBoost, float accelerationBoost, CameraState camState)
    {
        StartCoroutine(TemporalBoost(time, speedBoost + l_maxSpeed, accelerationBoost + l_acceleration, camState));
    }

    IEnumerator TemporalBoost(float time, float newSpeed, float newAcceleration, CameraState camState)
    {
        boosted = true;
         
        time -= Time.unscaledDeltaTime;
        l_maxSpeed = newSpeed;
        l_acceleration = newAcceleration;
        cam.ChangeState(camState);

        yield return new WaitForSeconds(time);

        l_maxSpeed = movementParameters.maxSpeed;
        l_acceleration = movementParameters.maxAcceleration;
        boosted = false;
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
}
