﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    //----------------------------------------------------- REFERENCES
    PlayerInput playerInput;
    CameraBehaviour cam;
    [HideInInspector] public Animator animator;
    //----------------------------------------------------------------

    //-------------------- SPACESHIP PROFILE -----------------------//
    public GameObject spaceship;
    SpaceshipProfile spaceshipProfile;
    //--------------------------------------------------------------//

    //---------------------------------------------- ENGINE PARAMETERS  
    [System.Serializable]
    public class EngineParameters
    {
        public float maxSpeed = 120;
        public float maxAcceleration = 40;
        public AnimationCurve accelerationCurve;
        public float maxBrake = 40;
        public AnimationCurve brakeCurve;      
    }

    [Header("ENGINE")]
    public EngineParameters engineParameters;

    //LOCAL
    [HideInInspector] public float l_maxSpeed, l_acceleration, 
    currentSpeed, horizontalMove, verticalMove, distanceTravelled = 0;
    [HideInInspector] public Vector3 forwardDirection = Vector3.left;
    //----------------------------------------------------------------

    //--------------------------------------------- CHASSIS PARAMETERS 
    [System.Serializable]
    public class ChasisParameters
    {
        public float maxHandlingSpeed = 15;
        public AnimationCurve handlingCurve;
        public float knockback = 10;
    }

    [Header("CHASIS")]
    public ChasisParameters chassisParameters;

    //LOCAL
    float stunTime = 0;
    //----------------------------------------------------------------

    //------------------------------------------------- JET PARAMETERS
    [System.Serializable]
    public class JetParameters
    {
        //NORMAL BOOST
        public float boostAcceleration = 2;
        public float boostConsumption = 2;
        
        //SUPERBOOST
        public float superBoostAcceleration = 2;
        public float superBoostConsumption = 2;
    }

    [Header("JETS")]
    public JetParameters jetParameters;

    //LOCAL
    bool nitroInputPhase, nitroInputReleased;
    float nitroInputInitialEnergy, l_nitroInputTime;

    [Tooltip("The window time to perform a superboost (Double tap 'Jet Button')")]
    public float superBoostInputTime = 0.4f;

    [HideInInspector] public bool boosted, superboosted;

    [HideInInspector] public GameObject[] jets;
    //----------------------------------------------------------------

    //--------------------------------------------- BLASTER PARAMETERS 
    [System.Serializable]
    public class BlasterParameters
    {
        public GameObject projectile;
        public float cadence = 0.2f;
    }

    [Header("BLASTER")]
    public BlasterParameters blasterParameters;

    //LOCAL
    float l_cadence = 0.2f;
    [HideInInspector] public Transform shotSpawn;
    //----------------------------------------------------------------

    //---------------------------------------------- ENERGY PARAMETERS
    [System.Serializable]
    public class EnergyParameters
    {
        public float maxEnergy = 300;
        public float initialEnergy = 0;
    }

    [Header("ENERGY TANK")]
    public EnergyParameters energyParameters;

    //LOCAL
    [HideInInspector] public float l_energy;
    //----------------------------------------------------------------

    //----------------------------------------------------- LAPCHECKER
    bool endedLap;

    [Header("SETTINGS")]
    public float initialDistance;

    private void Awake()
    {
        GameManager.Instance.player = this;
    }

    private void Start()
    {
        //GET PARAMETERS FROM MENU IF SELECTED SPACESHIP ON GAME MANAGER IS NOT NULL
        if (GameManager.Instance.selectedSpaceship != null)
            spaceship = GameManager.Instance.selectedSpaceship;

        //INSTANTIATE SPACESHIP
        spaceship = Instantiate(spaceship, transform.position, transform.rotation);
        spaceship.transform.parent = transform;
        spaceship.name = spaceship.name.Replace("(Clone)", "");
        spaceship.GetComponent<SpaceshipStructure>().profile = (SpaceshipProfile)AssetDatabase.LoadAssetAtPath("Assets/Scripts/Scriptables/Spaceships/" + spaceship.name + ".asset", typeof(SpaceshipProfile));

        //GET REFERENCES
        playerInput = GetComponent<PlayerInput>();
        cam = GameManager.Instance.playerCamera;
        spaceshipProfile = spaceship.GetComponent<SpaceshipStructure>().profile;

        //INITIALIZE SPACESHIP PARAMETERS
        if (spaceshipProfile != null)
        {
            //-----------------------------------------------------------------------
            //----------------------------------------------------------- FROM ENGINE
            if (spaceshipProfile.engineProfile != null)
            {
                // SPEED
                engineParameters.maxSpeed = spaceshipProfile.engineProfile.maxSpeed;

                // ACCELERATION
                engineParameters.maxAcceleration = spaceshipProfile.engineProfile.maxAcceleration;
                engineParameters.accelerationCurve = spaceshipProfile.engineProfile.accelerationCurve;

                // BRAKE
                engineParameters.maxBrake = spaceshipProfile.engineProfile.maxBrake;
                engineParameters.brakeCurve = spaceshipProfile.engineProfile.brakeCurve;
            }
            else Debug.LogWarning("There is no Engine Profile on " + spaceshipProfile.name + ". Loading default parameters...");

            //-----------------------------------------------------------------------
            //---------------------------------------------------------- FROM CHASSIS
            if (spaceshipProfile.chassisProfile != null)
            {
                // HANDLING
                chassisParameters.maxHandlingSpeed = spaceshipProfile.chassisProfile.handling;
                chassisParameters.handlingCurve = spaceshipProfile.chassisProfile.handlingCurve;

                // RESISTANCE
                chassisParameters.knockback = spaceshipProfile.chassisProfile.knockback;
            }
            else Debug.LogWarning("There is no Chassis Profile on " + spaceshipProfile.name + ". Loading default parameters...");

            //-----------------------------------------------------------------------
            //--------------------------------------------------------- FROM BLASTERS
            if (spaceshipProfile.blasterProfile != null)
            {
                // PROJECTILE
                blasterParameters.projectile = spaceshipProfile.blasterProfile.projectile;

                // CADENCE
                blasterParameters.cadence = spaceshipProfile.blasterProfile.cadence;
            }
            else Debug.LogWarning("There is no Blaster Profile on " + spaceshipProfile.name + ". Loading default parameters...");

            //-----------------------------------------------------------------------
            //------------------------------------------------------------- FROM JETS
            if (spaceshipProfile.jetProfile != null)
            {
                //BOOST
                jetParameters.boostAcceleration = spaceshipProfile.jetProfile.boost;
                jetParameters.boostConsumption = spaceshipProfile.jetProfile.boostIntake;

                //SUPERBOOST
                jetParameters.superBoostAcceleration = spaceshipProfile.jetProfile.superBoost;
                jetParameters.superBoostConsumption = spaceshipProfile.jetProfile.superBoostIntake;
            }
            else Debug.LogWarning("There is no Jet Profile on " + spaceshipProfile.name + ". Loading default parameters...");

            //-----------------------------------------------------------------------
            //------------------------------------------------------------- FROM TANK
            if (spaceshipProfile.tankProfile != null)
            {
                // ENERGY TANK CAPACITY
                energyParameters.maxEnergy = spaceshipProfile.tankProfile.capacity;
            }
            else Debug.LogWarning("There is no Tank Profile on " + spaceshipProfile.name + ". Loading default parameters...");
        }

        else Debug.Log("There isn't any profile to load. Initializing with default parameters...");

        //INITIALIZE LOCAL VARIABLES     
        l_maxSpeed = engineParameters.maxSpeed;
        l_energy = energyParameters.initialEnergy;
        l_cadence = blasterParameters.cadence;
        l_nitroInputTime = superBoostInputTime;

        //SET INITIAL POSITION
        initialDistance = 0;
        transform.position = TrackManager.Instance.GetPositionAtDistance(initialDistance);
        distanceTravelled = initialDistance;

        //GET FORWARD VECTOR
        forwardDirection = TrackManager.Instance.GetDirectionAtDistance(distanceTravelled);
        transform.forward = forwardDirection.normalized;
    }

    void Update()
    {

        //STARTING SEQUENCE
        if (distanceTravelled < 60)
        {
            StartingSequence();
        }
        else
        {
            UIManager.Instance.UIW.countDown.enabled = true;
            RaceManager.Instance.startSeqEnded = true;
        }
        if (playerInput.inputEnabled)
        {
            //MOVEMENT
            UpdateMove();

            //NITRO 
            UpdateNitro();

            //BARREL ROLL
            UpdateRoll();

            //BLASTERS
            UpdateBlasters();
        }
        
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
        animator.SetBool("Knockback", false);
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
                l_acceleration = (engineParameters.brakeCurve.Evaluate(currentSpeed / l_maxSpeed) * engineParameters.maxBrake);
               
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
                l_acceleration = (engineParameters.accelerationCurve.Evaluate(currentSpeed / l_maxSpeed) * engineParameters.maxAcceleration);

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
                    currentSpeed -= engineParameters.maxAcceleration * Time.deltaTime;
                }

                //CHANGE CAMERA
                if (cam.cameraState != CameraState.moving && !boosted && !superboosted)
                    cam.ChangeState(CameraState.moving);

                //SET ANIMATOR
                animator.SetBool("Brake", false);
            }

            //IDLE
            else
            {
                //DECELERATE
                currentSpeed -= engineParameters.maxAcceleration * Time.deltaTime;

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

        //CALCULATE 2D MOVEMENT VALUE WITH HANDLING CURVE
        float handling = chassisParameters.maxHandlingSpeed * chassisParameters.handlingCurve.Evaluate(currentSpeed / l_maxSpeed);
        horizontalMove += playerInput.rawMovement.x * handling * Time.deltaTime;
        verticalMove += playerInput.rawMovement.y * handling * Time.deltaTime;

        //CLAMP ON LIMITS
        horizontalMove = Mathf.Clamp(horizontalMove, -TrackManager.Instance.movementLimits.x, TrackManager.Instance.movementLimits.x);
        verticalMove = Mathf.Clamp(verticalMove, -TrackManager.Instance.movementLimits.y, TrackManager.Instance.movementLimits.y);

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
                        OneShotBoost(l_energy / jetParameters.superBoostConsumption, 
                            jetParameters.superBoostAcceleration, true, CameraState.superboost);
                        l_energy = 0;
                    }
                }
            }

            //IF INPUT TIME ENDS, END INPUT PHASE
            else
            {
                nitroInputPhase = false;
                l_nitroInputTime = superBoostInputTime;
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
        if (animator != null)
        {
            if (playerInput.roll)
            {
                //SET ANIMATOR
                animator.SetBool("Brake", false);

                //LEFT
                if (playerInput.rawMovement.x < 0)
                    animator.SetBool("Roll_Left", true);

                //RIGHT
                else if (playerInput.rawMovement.x > 0)
                    animator.SetBool("Roll_Right", true);

                //STRAIGHT
                else
                {
                    if (Random.value < 0.5f) animator.SetBool("Roll_Left", true);
                    else animator.SetBool("Roll_Right", true);
                }
            }

            else
            {
                animator.SetBool("Roll_Left", false);
                animator.SetBool("Roll_Right", false);
            }
        }
    }

    void UpdateBlasters()
    {
        if (playerInput.blaster)
        {
            l_cadence -= Time.deltaTime;
            if (l_cadence < 0)
            {
                GameObject s = Instantiate(blasterParameters.projectile, shotSpawn.position, shotSpawn.rotation);
                s.tag = "Player";
                s.GetComponent<ProjectileParameters>().initialSpeed = currentSpeed;
                l_cadence = blasterParameters.cadence;
            }
        }
    }

    public void Boost(float accelerationBoost, CameraState camState)
    {       
        currentSpeed += accelerationBoost * Time.deltaTime;
        if (cam.cameraState != camState)
            cam.ChangeState(camState);
    }

    public void OneShotBoost(float duration, float accelerationBoost, bool energyCost, CameraState camState)
    {
        StartCoroutine(BoostCoroutine(duration, accelerationBoost, energyCost, camState));
    }

    IEnumerator BoostCoroutine(float duration, float accelerationBoost, bool energyCost, CameraState camState)
    {
        superboosted = true;
        while (duration > 0)
        {
            duration -= Time.deltaTime;

            currentSpeed += accelerationBoost * Time.deltaTime;

            if(energyCost) l_energy -= jetParameters.superBoostConsumption * Time.deltaTime;

            if (cam.cameraState != camState)
                cam.ChangeState(camState);

            if (energyCost) l_energy = 0;

            yield return null;
        }
        superboosted = false;
        cam.ChangeState(CameraState.moving);
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

    public void Knockback()
    {
        currentSpeed -= chassisParameters.knockback;
        stunTime = 0.5f;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
        animator.SetBool("Knockback", true);
    }

    public void Explode()
    {
        this.enabled = false;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
    }

    public void StartingSequence()
    {
       
        Vector3 movementDirection = TrackManager.Instance.GetPositionAtDistance(distanceTravelled);

        transform.position = movementDirection;
        distanceTravelled += 0.05f;

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
                Knockback();
                break;

            case "HardObstacle":
                Knockback();
                break;

            case "Enemy":
                Knockback();
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
