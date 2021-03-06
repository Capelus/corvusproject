﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerBehaviour : MonoBehaviour
{
    //----------------------------------------------------- REFERENCES
    PlayerInput playerInput;
    [HideInInspector] public Animator animator;
    //----------------------------------------------------------------

    //-------------------- SPACESHIP PROFILE -----------------------//
    public GameObject spaceship;
    SpaceshipProfile spaceshipProfile;
    UpgradesProfile upgradesProfile;
    //--------------------------------------------------------------//

    //----------------------------------- SPACESHIP EFFECTS REFERENCES
    [HideInInspector] public GameObject[] jets;
    [HideInInspector] public GameObject[] windTrails;
    [HideInInspector] public GameObject[] sparks;
    [HideInInspector] public GameObject[] overchargeTrails;
    //----------------------------------------------------------------

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
        public float limitSpeed = 500;
        public float maxHandlingSpeed = 15;
        public AnimationCurve handlingCurve;
        public float knockback = 10;
        public float handlingRollMultiplier = 10;
    }

    [Header("CHASIS")]
    public ChasisParameters chassisParameters;

    //LOCAL
    float stunTime = 0;
    bool rolling = false;
    float l_handlingRollMultiplier;
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

    [Tooltip("The time window to perform a superboost (Double tap 'Jet Button')")]
    public float superBoostInputTime = 0.4f;

    [HideInInspector] public bool boosted, superboosted, outBoosted;
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
    Vector3 movement;
    [HideInInspector] public float l_energy;
    //----------------------------------------------------------------

    //----------------------------------------------- COLLISION SYSTEM
    Ray rayUp, rayDown, rayLeft, rayRight;
    bool hitUp, hitDown, hitLeft, hitRight;
    //----------------------------------------------------------------

    //-------------------------------------------------------------FOG
    bool isInsideFog;
    //-----------------------------------------------------LAP CHECKER
    bool endedLap;
    //----------------------------------------------------------------

    [Header("SETTINGS")]
    public float initialDistance;
    public float initialVerticalOffset;
    public float initialHorizontalOffset;

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
        spaceship.GetComponent<SpaceshipStructure>().profile = Resources.Load<SpaceshipProfile>("Scriptables/Spaceships/" + spaceship.name);

        //GET REFERENCES
        playerInput = GetComponent<PlayerInput>();
        spaceshipProfile = spaceship.GetComponent<SpaceshipStructure>().profile;
        upgradesProfile = spaceship.GetComponent<SpaceshipStructure>().upgradesProfile;

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

                if(upgradesProfile.engineUpgrades != null)
                {
                    engineParameters.maxSpeed += upgradesProfile.engineUpgrades.bonusMaxSpeed;
                    engineParameters.maxAcceleration += upgradesProfile.engineUpgrades.bonusMaxAcceleration;
                    engineParameters.maxBrake += upgradesProfile.engineUpgrades.bonusMaxBrake;
                }

            }
            else Debug.LogWarning("There is no Engine Profile on " + spaceshipProfile.name + ". Loading default parameters...");

            //-----------------------------------------------------------------------
            //---------------------------------------------------------- FROM CHASSIS
            if (spaceshipProfile.chassisProfile != null)
            {
                // LIMIT SPEED
                chassisParameters.limitSpeed = spaceshipProfile.chassisProfile.limitSpeed;

                // HANDLING
                chassisParameters.maxHandlingSpeed = spaceshipProfile.chassisProfile.handling;
                chassisParameters.handlingCurve = spaceshipProfile.chassisProfile.handlingCurve;

                // ROLL DASH
                chassisParameters.handlingRollMultiplier = spaceshipProfile.chassisProfile.handlingRollMultiplier;

                // RESISTANCE
                chassisParameters.knockback = spaceshipProfile.chassisProfile.knockback;

                if (upgradesProfile.chassisUpgrades != null)
                {
                    chassisParameters.maxHandlingSpeed += upgradesProfile.chassisUpgrades.bonusMaxHandling;
                    chassisParameters.knockback += upgradesProfile.chassisUpgrades.bonusKnockback;
                }
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

                if (upgradesProfile.blastersUpgrades != null)
                {
                    blasterParameters.cadence += upgradesProfile.blastersUpgrades.bonusCadence;
                }
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

                if (upgradesProfile.jetsUpgrades != null)
                {
                    jetParameters.boostAcceleration += upgradesProfile.jetsUpgrades.bonusMaxBoost;
                    jetParameters.superBoostAcceleration += upgradesProfile.jetsUpgrades.bonusMaxSuperBoost;
                    jetParameters.boostConsumption += upgradesProfile.jetsUpgrades.bonusBoostIntake;
                    jetParameters.superBoostConsumption += upgradesProfile.jetsUpgrades.bonusSuperBoostIntake;
                }
            }
            else Debug.LogWarning("There is no Jet Profile on " + spaceshipProfile.name + ". Loading default parameters...");

            //-----------------------------------------------------------------------
            //------------------------------------------------------------- FROM TANK
            if (spaceshipProfile.tankProfile != null)
            {
                // ENERGY TANK CAPACITY
                energyParameters.maxEnergy = spaceshipProfile.tankProfile.capacity;

                if (upgradesProfile.tanksUpgrades != null)
                {
                    energyParameters.maxEnergy += upgradesProfile.tanksUpgrades.bonusCapacity;
                }
            }
            else Debug.LogWarning("There is no Tank Profile on " + spaceshipProfile.name + ". Loading default parameters...");
        }

        else Debug.Log("There isn't any profile to load. Initializing with default parameters...");

        //INITIALIZE LOCAL VARIABLES     
        l_maxSpeed = engineParameters.maxSpeed;
        l_energy = energyParameters.initialEnergy;
        l_cadence = blasterParameters.cadence;
        l_nitroInputTime = superBoostInputTime;
        l_handlingRollMultiplier = 1;

        //SET INITIAL POSITION
        transform.position = TrackManager.Instance.GetPositionAtDistance(initialDistance) + transform.up * initialVerticalOffset + transform.right * initialHorizontalOffset;
        distanceTravelled = initialDistance;
        horizontalMove = initialHorizontalOffset;
        verticalMove = initialVerticalOffset;

        //SET FORWARD VECTOR
        forwardDirection = TrackManager.Instance.GetDirectionAtDistance(distanceTravelled);
        transform.forward = forwardDirection.normalized;

        //SET FOG INITIAL VALUE TO FALSE
        isInsideFog = false;

        //SET LAPCHECKER
        endedLap = false;
    }

    void Update()
    {
        if (playerInput.movementEnabled)
        {
            //MOVEMENT
            UpdateMove();

            if (playerInput.inputEnabled)
            {
                //NITRO 
                UpdateNitro();

                //BARREL ROLL
                UpdateRoll();

                //BLASTERS
                UpdateBlasters();
            }
        }

        UpdateEffects();
        
        //CLAMP ENERGY
        l_energy = Mathf.Clamp(l_energy, 0, energyParameters.maxEnergy);

        //--------------- DEBUG
        if (playerInput.rechargeEnergy) //RECHARGE ENERGY
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
        stunTime -= Time.deltaTime;
        if (stunTime <= 0)
        {
            //BRAKE
            if (playerInput.inputEnabled && playerInput.brake)
            {
                //CALCULATE BRAKE VALUE ON CURVE
                l_acceleration = (engineParameters.brakeCurve.Evaluate(currentSpeed / l_maxSpeed) * engineParameters.maxBrake);
               
                //APPLY THROTTLE
                l_acceleration *= playerInput.throttle;
                
                //BRAKE
                currentSpeed += l_acceleration * Time.deltaTime;

                //SET ANIMATOR
                animator.SetBool("Brake", true);

                //CHANGE CAMERA STATE
                GameManager.Instance.playerCamera.ChangeState(CameraState.braking);
            }

            //ACCELERATE (EXCEPT IF PLAYER IS INSIDE FOG)
            else if (playerInput.accelerate && !isInsideFog && playerInput.inputEnabled)
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
                if(currentSpeed > l_maxSpeed + (l_maxSpeed * 0.2f) && !boosted && !superboosted && !outBoosted)
                {
                    currentSpeed -= engineParameters.maxAcceleration * Time.deltaTime;
                }

                //SET ANIMATOR
                animator.SetBool("Brake", false);

                //CHANGE CAMERA STATE
                if(!superboosted)
                    GameManager.Instance.playerCamera.ChangeState(CameraState.moving);
            }

            //IDLE
            else
            {
                //DECELERATE
                currentSpeed -= engineParameters.maxAcceleration * Time.deltaTime;

                //SET ANIMATOR
                animator.SetBool("Brake", false);

                //CHANGE CAMERA STATE
                GameManager.Instance.playerCamera.ChangeState(CameraState.idle);
            }

            //RESTORE STUN EFFECT (Esta un poco feo aquí pero bueno, ya se hará un manager de efectos de cámara en condiciones!)
            GameManager.Instance.playerCamera.ModifyPostproEffect("ChromaticAberration", 0);
            UIManager.Instance.UI.engineWarning.SetActive(false);
        }

        //CLAMP SPEED
        currentSpeed = Mathf.Clamp(currentSpeed, 5, chassisParameters.limitSpeed);

        //INCREMENT DISTANCE TRAVELLED
        distanceTravelled += currentSpeed * Time.deltaTime;

        //CALCULATE 2D MOVEMENT VALUE WITH HANDLING CURVE
        float handling = chassisParameters.maxHandlingSpeed * chassisParameters.handlingCurve.Evaluate(currentSpeed / l_maxSpeed);

        if (stunTime > 0)
            handling /= 3;

        //GET PLAYER'S INPUT
        float inputX = playerInput.rawMovement.x;
        float inputY = playerInput.rawMovement.y;

        //LOCK ON COLLISIONS
        if (this.hitUp)
            if (inputY > 0) inputY = 0;

        if (this.hitDown)
            if (inputY < 0) inputY = 0;

        if (this.hitLeft)
            if (inputX < 0) inputX = 0;

        if (this.hitRight)
            if (inputX > 0) inputX = 0;

        //CALCULATE 2D MOVEMENT
        if (playerInput.inputEnabled)
        {
            horizontalMove += inputX * handling * l_handlingRollMultiplier * Time.deltaTime;
            verticalMove += inputY * handling * l_handlingRollMultiplier * Time.deltaTime;
        }

        //CLAMP IT ON LIMITS
        horizontalMove = Mathf.Clamp(horizontalMove, -TrackManager.Instance.movementLimits.x, TrackManager.Instance.movementLimits.x);
        verticalMove = Mathf.Clamp(verticalMove, -TrackManager.Instance.movementLimits.y, TrackManager.Instance.movementLimits.y);

        //CALCULATE DESIRED MOVEMENT
        movement = TrackManager.Instance.GetPositionAtDistance(distanceTravelled);
        movement += transform.right * horizontalMove;
        movement += transform.up * verticalMove;

        //CHECK COLLISIONS
        CheckCollisions();

        //FINALLY, MOVE
        transform.position = movement;

        //ROTATE
        Quaternion targetRotation = TrackManager.Instance.GetRotationAtDistance(distanceTravelled);
        var step = 150 * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);

        //TILT
        if (playerInput.inputEnabled)
        {
        animator.SetFloat("Tilt X", playerInput.smoothedMovement.x);
        animator.SetFloat("Tilt Y", playerInput.smoothedMovement.y);
        }
    }

    void UpdateNitro()
    {
        boosted = false;

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
            l_nitroInputTime -= Time.deltaTime;
            if (l_nitroInputTime > 0)
            {
                //IF THIS IS NOT THE FIRST FRAME INPUT...
                if (nitroInputReleased)
                {
                    //DETECT SECOND INPUT FOR SUPER BOOST IF NITRO CHARGE IS FULL
                    if (playerInput.nitroPress && nitroInputInitialEnergy == energyParameters.maxEnergy)
                    {
                        //SUPERBOOST
                        OneShotBoost(nitroInputInitialEnergy / jetParameters.superBoostConsumption, 
                            jetParameters.superBoostAcceleration, true, CameraState.superboost);
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
            if (l_energy > 0) //IF THERE IS ENERGY...
                Boost(jetParameters.boostAcceleration, true, CameraState.boost);
            else boosted = false;
        else boosted = false;
    }

    void UpdateRoll()
    {
        if (playerInput.roll && !rolling)
            StartCoroutine("RollCoroutine");

        else
        {
            animator.SetBool("Roll_Left", false);
            animator.SetBool("Roll_Right", false);
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

    void UpdateEffects()
    {
        // UPDATE JET SIZE
        foreach (GameObject jet in jets)
        {
            jet.transform.localScale = Vector3.one * Mathf.Clamp(playerInput.throttle, 0.5f, 1);
        }

        // UPDATE WINDTRAIL DISSOLVE
        foreach (GameObject windTrail in windTrails)
            windTrail.GetComponent<TrailRenderer>().material.SetFloat("Dissolve_", Mathf.Clamp(1 - (currentSpeed / engineParameters.maxSpeed), 0.2f, 0.8f));        
    }

    public void Boost(float accelerationBoost, bool energyCost, CameraState camState)
    {       
        if(energyCost)
            l_energy -= jetParameters.boostConsumption * Time.deltaTime;

        boosted = true;
        currentSpeed += accelerationBoost * Time.deltaTime;
        GameManager.Instance.playerCamera.ChangeState(camState);
    }

    public void OneShotBoost(float duration, float accelerationBoost, bool energyCost, CameraState camState)
    {
        StartCoroutine(BoostCoroutine(duration, accelerationBoost, energyCost, camState));
    }

    IEnumerator BoostCoroutine(float duration, float accelerationBoost, bool energyCost, CameraState camState)
    {
        superboosted = true;
        GameManager.Instance.playerCamera.ChangeState(camState);

        foreach (GameObject overchargeTrail in overchargeTrails)
            overchargeTrail.SetActive(true);
        
        while (duration > 0)
        {
            duration -= Time.deltaTime;

            currentSpeed += accelerationBoost * Time.deltaTime;

            if(energyCost) l_energy -= jetParameters.superBoostConsumption * Time.deltaTime;

            yield return null;
        }

        superboosted = false;
        foreach (GameObject overchargeTrail in overchargeTrails)
            overchargeTrail.SetActive(false);
    }

    IEnumerator RollCoroutine()
    {
        //BOOST HANDLING
        l_handlingRollMultiplier = chassisParameters.handlingRollMultiplier;

        //ANIMATE
        if (animator != null)
        {
            animator.SetBool("Brake", false); //RESET ANIMATOR

            if (playerInput.rawMovement.x < 0)  //LEFT
                animator.SetBool("Roll_Left", true);

            else if (playerInput.rawMovement.x > 0) //RIGHT
                animator.SetBool("Roll_Right", true);

            else //STRAIGHT
            {
                if (Random.value < 0.5f) animator.SetBool("Roll_Left", true);
                else animator.SetBool("Roll_Right", true);
            }
        }

        float t = 0.3f; // The time while handling is boosted. Currently hardcoded because it is not clear if we are gonna use it.
        yield return new WaitForSeconds(t);

        // RESTORE HANDLING
        l_handlingRollMultiplier = 1;
    }

    public void CheckCollisions()
    {
        rayUp = new Ray(movement, transform.up);
        rayDown = new Ray(movement, -transform.up);
        rayLeft = new Ray(movement, -transform.right);
        rayRight = new Ray(movement, transform.right);

        if (Physics.Raycast(rayUp, out RaycastHit hitUp, 1))
        {
            if (hitUp.transform.CompareTag("Wall"))
            {
                Vector3 prevMovement = movement;
                movement -= transform.up * (1 - (Vector3.Distance(hitUp.point, movement)));
                verticalMove += (movement - prevMovement).y;
                this.hitUp = true;
            }
        }
        else this.hitUp = false;

        if (Physics.Raycast(rayDown, out RaycastHit hitDown, 1))
        {
            if (hitDown.transform.CompareTag("Wall"))
            {
                Vector3 prevMovement = movement;
                movement += transform.up * (1 - (Vector3.Distance(hitDown.point, movement)));
                verticalMove += (movement - prevMovement).y;
                this.hitDown = true;
            }
        }
        else this.hitDown = false;

        if (Physics.Raycast(rayLeft, out RaycastHit hitLeft, 2.3f))
        {
            if (hitLeft.transform.CompareTag("Wall"))
            {
                Vector3 prevMovement = movement;
                movement += transform.right * (2.3f - (Vector3.Distance(hitLeft.point, movement)));

                Vector3 offsetMovement = (movement - prevMovement);
                offsetMovement.y = 0;
                horizontalMove += offsetMovement.magnitude;
                this.hitLeft = true;

                //EFFECTS
                sparks[0].SetActive(true);
            }
        }
        else
        {
            this.hitLeft = false;
            sparks[0].SetActive(false);
        }

        if (Physics.Raycast(rayRight, out RaycastHit hitRight, 2.3f))
        {
            if (hitRight.transform.CompareTag("Wall"))
            {
                Vector3 prevMovement = movement;
                movement -= transform.right * (2.3f - (Vector3.Distance(hitRight.point, movement)));

                Vector3 offsetMovement = (movement - prevMovement);
                offsetMovement.y = 0;
                horizontalMove -= offsetMovement.magnitude;
                this.hitRight = true;

                //EFFECTS
                sparks[1].SetActive(true);
            }
        }
        else
        {
            this.hitRight = false;
            sparks[1].SetActive(false);
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

    public void Knockback()
    {
        currentSpeed -= chassisParameters.knockback;
        stunTime = 0.5f;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
        animator.SetBool("Knockback", true);
    }

    public void Stun(float time)
    {
        stunTime = time;
        GameManager.Instance.playerCamera.ModifyPostproEffect("ChromaticAberration", 1);
        UIManager.Instance.UI.engineWarning.SetActive(true);
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
                    GameManager.Instance.playerCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("PitLane"));
                    GameManager.Instance.playerCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("RaceTrack");
                }

                else
                {
                    GameManager.Instance.playerCamera.GetComponent<Camera>().cullingMask |= 1 << LayerMask.NameToLayer("PitLane");
                    GameManager.Instance.playerCamera.GetComponent<Camera>().cullingMask &= ~(1 << LayerMask.NameToLayer("RaceTrack"));
                }
                break;

            case "Fog":
                isInsideFog = true;
                break;

            case "Finish":
                if (!endedLap)
                {
                    RaceManager.Instance.LapChecker();
                }
                endedLap = true;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Boost":
                Boost(jetParameters.boostAcceleration, false, CameraState.boost);
                RechargeEnergy(10);
                outBoosted = true;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            //------------------------------------FOG
            case "Fog":
                isInsideFog = false;
                break;

            case "Boost":
                boosted = false;
                outBoosted = false;
                break;

            case "Finish":
                endedLap = false;
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
                if (collision.transform.GetComponent<BreakableBehaviour>())
                    collision.transform.GetComponent<BreakableBehaviour>().Destroy();
                break;

            case "Enemy":
                Knockback();
                break;
        }
    }
}
