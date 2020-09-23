using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTestingScript : MonoBehaviour
{
    //VELOCITY
    public float maxVelocity = 50;
    float currentVelocity = 0;

    //ACCELERATION
    public float maxAcceleration = 20;
    public float accelerationTime;
    public AnimationCurve accelerationCurve;
    float currentAcceleration;

    Vector3 startPosition;
    Vector3 movement;

    float t = 0;

    bool move;

    //DEBUG
    TextMesh text;

    void Start()
    {
        startPosition = transform.position;

        //DEBUG
        text = GetComponentInChildren<TextMesh>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            move = !move;
        
        if (Input.GetKeyDown(KeyCode.R))
            ResetPosition();
        
        if (move)
            Move();

        //DEBUG
        text.text = currentAcceleration.ToString("0.00");
    }

    void Move()
    {
        //GET THE ACCELERATION VALUE FROM CURVE
        t += Time.deltaTime;
        currentAcceleration = accelerationCurve.Evaluate(t / accelerationTime);

        //ADD ACCELERATION TO THE OBJECT'S VELOCITY
        currentVelocity += currentAcceleration * maxAcceleration * Time.deltaTime;

        //CLAMP VELOCITY ON ITS LIMITS
        currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);

        //APPLY THE MOVEMENT TO THE OBJECT
        movement = transform.forward * currentVelocity;
        transform.position += movement * Time.deltaTime;
    }

    void ResetPosition()
    {
        transform.position = startPosition;
        currentVelocity = 0;
        t = 0;
        move = false;
    }
}
