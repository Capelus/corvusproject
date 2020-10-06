using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //REFERENCES
    PlayerInput input;
    SpaceshipProfile profile;
    Animator animator;

    //PARAMETERS
    float maxSpeed;
    float maxAcceleration;
    float maxBrake;
    AnimationCurve accelerationCurve;
    AnimationCurve brakeCurve;

    float handling;
    AnimationCurve handlingCurve;


    float currentSpeed;

    float t = 0;
    float damp = 0.3f;

    Vector3 rotation = Vector3.zero;

    void Start()
    {
        //GET REFERENCES
        //rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
        profile = GetComponentInChildren<SpaceshipStructure>().profile;
        animator = GetComponentInChildren<Animator>();

        Debug.Log(profile.name);

        //INITIALIZE PARAMTERES
        maxSpeed = profile.engineProfile.maxSpeed;
        maxAcceleration = profile.engineProfile.maxAcceleration;
        maxBrake = profile.engineProfile.maxBrake;
        accelerationCurve = profile.engineProfile.accelerationCurve;
        brakeCurve = profile.engineProfile.brakeCurve;

        handling = profile.chassisProfile.handling;
        handlingCurve = profile.chassisProfile.handlingCurve;
            
    }

    
    void Update()
    {
        Move();
    }

    void Move()
    {
        t += Time.deltaTime * damp;

        //ACCELERATE
        Vector3 movement = Vector3.zero;

        movement += transform.forward * (accelerationCurve.Evaluate(currentSpeed / maxSpeed) * maxAcceleration) * input.throttle;

        transform.Rotate(Vector3.up * (handlingCurve.Evaluate(currentSpeed / maxSpeed) * handling) * input.rawMovement.x * Time.deltaTime, Space.World);
        transform.Rotate(transform.right * (handlingCurve.Evaluate(currentSpeed / maxSpeed) * handling) * input.rawMovement.y * Time.deltaTime, Space.World);

        animator.SetFloat("Tilt X", input.rawMovement.x);
        animator.SetFloat("Tilt Y", -input.rawMovement.y);

        //TURN
        //rotation += transform.up * (handlingCurve.Evaluate(currentSpeed / maxSpeed) * handling) * input.smoothedMovement.x;
        //rotation += transform.right * (handlingCurve.Evaluate(currentSpeed / maxSpeed) * handling) * input.smoothedMovement.y;

        transform.position += movement * Time.deltaTime;
        //transform.localRotation *= Quaternion.Euler(rotation * Time.deltaTime);
    }
}
