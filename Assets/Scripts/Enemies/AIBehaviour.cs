using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public SpaceshipProfile profile;

    float currentSpeed;
    float acceleration;

    public float initialDistance;
    float distanceTravelled = 0;

    float horizontalMove = 0;
    float verticalMove = 0;
    float horizontalDir = 0;
    float verticalDir = 0;
    public float changeDirectionTimer = 3;
    float changeDirTimer;

    Vector3 forwardDirection;

    bool superboosted;

    Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        changeDirTimer = changeDirectionTimer;

        //SET INITIAL POSITION
        transform.position = TrackManager.Instance.GetPositionAtDistance(initialDistance) 
            + transform.right * Random.Range(- TrackManager.Instance.movementLimits.x, TrackManager.Instance.movementLimits.x)
            + transform.up * Random.Range(-TrackManager.Instance.movementLimits.y, TrackManager.Instance.movementLimits.y);
        distanceTravelled = initialDistance;

        //GET FORWARD VECTOR
        forwardDirection = TrackManager.Instance.GetDirectionAtDistance(distanceTravelled);
        transform.forward = forwardDirection.normalized;
    }

    void Update()
    {
        if (RaceManager.Instance.raceStarted)
        {
            Move();
        }
    }

    void Move()
    {
        //GET ACCELERATION VALUE FROM CURVE
        acceleration = (profile.engineProfile.accelerationCurve.Evaluate(currentSpeed / profile.engineProfile.maxSpeed) * profile.engineProfile.maxAcceleration);

        //INCREASE SPEED
        while (Mathf.Abs(currentSpeed - profile.engineProfile.maxSpeed) > Mathf.Epsilon)
        {
            currentSpeed += acceleration * Time.deltaTime;
            break;
        }

        //REDUCE SPEED WHEN OVERLIMIT & NOT BOOSTED
        if (currentSpeed > profile.engineProfile.maxSpeed + (profile.engineProfile.maxSpeed * 0.1f) && !superboosted)
        {
            currentSpeed -= profile.engineProfile.maxAcceleration * Time.deltaTime;
        }

        //CLAMP SPEED
        currentSpeed = Mathf.Clamp(currentSpeed, 5, 500);

        //INCREMENT DISTANCE TRAVELLED
        distanceTravelled += currentSpeed * Time.unscaledDeltaTime;

        //CALCULATE RANDOM DIRECTION
        changeDirTimer -= Time.deltaTime;
        if (changeDirTimer < 0)
        {
            horizontalDir = Random.Range(-0.1f, 0.1f);
            verticalDir = Random.Range(-0.1f, 0.1f);
            changeDirTimer = changeDirectionTimer;
        }

        //CALCULATE 2D MOVEMENT VALUE WITH HANDLING CURVE
        float handling = profile.chassisProfile.handling * profile.chassisProfile.handlingCurve.Evaluate(currentSpeed / profile.engineProfile.maxSpeed);
        horizontalMove += horizontalDir * handling * Time.deltaTime;
        verticalMove += verticalDir * handling * Time.deltaTime;

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
        transform.forward = TrackManager.Instance.GetPositionAtDistance(distanceTravelled+5) - transform.position;
        //Quaternion targetRotation = TrackManager.Instance.GetRotationAtDistance(distanceTravelled);
        //var step = 150 * Time.deltaTime;
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);

        //TILT
        animator.SetFloat("Tilt X", horizontalMove);
        animator.SetFloat("Tilt Y", verticalMove);
    }

    public void OneShotBoost(float duration, float accelerationBoost)
    {
        StartCoroutine(BoostCoroutine(duration, accelerationBoost));
    }

    IEnumerator BoostCoroutine(float duration, float accelerationBoost)
    {
        superboosted = true;
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            currentSpeed += accelerationBoost * Time.deltaTime;

            yield return null;
        }
        superboosted = false;
        yield break;
    }

    public void Knockback()
    {
        currentSpeed -= profile.chassisProfile.knockback;
        //stunTime = 0.5f;
        EffectsManager.Instance.InstantiateEffect("Explosion", transform.position, transform.rotation);
        animator.SetBool("Knockback", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            //------------------------------------------ ENERGY RING
            case "Nitro":
                SoundManager.Instance.PlaySound("NitroCluster");
                Destroy(other.gameObject);
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
        }
    }
}
