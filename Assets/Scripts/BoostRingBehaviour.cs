using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRingBehaviour : MonoBehaviour
{
    //REFERENCES
    GameObject ring;
    CapsuleCollider trigger;

    public float rotationSpeed = 50;
    public float skillcheckRotationSpeed = 50;
    public float skillcheckDuration = 5;
    public float slowTimeFactor = 4;
    public float cooldownTime = 2;
    float l_cooldownTime = 0;

    //BOOST PARAMETERS
    [System.Serializable]
    public class BoostParameters
    {
        public float failBoost;
        public float greatBoost;
        public float perfectBoost;
    }
    public BoostParameters boostParameters;

    float ringDistance;
    float previousSpeed;
    float boost = 0;

    float zAngle;

    bool coroutineStarted = false;

    private void Start()
    {
        ring = transform.GetChild(0).gameObject;
        trigger = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        //ROTATE
        ring.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        l_cooldownTime -= Time.unscaledDeltaTime;
        if (l_cooldownTime < 0 && !trigger.enabled)
            trigger.enabled = true;
    }

    //SKILLCHECK COROUTINE
    IEnumerator SkillCheck()
    {
        coroutineStarted = true;

        while (!GameManager.Instance.playerInput.roll)
        {
            ring.transform.Rotate(0, 0, skillcheckRotationSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        zAngle = ring.transform.rotation.normalized.eulerAngles.z;

        //GIVE BOOST
        if (zAngle > 100 && zAngle < 120 || zAngle > 220 && zAngle < 240 || zAngle > 340 && zAngle < 360) //PERFECT
        {
            Debug.Log("PERFECT");
            boost = boostParameters.perfectBoost;
        }

        else if (zAngle > 60 && zAngle < 100 || zAngle > 180 && zAngle < 220 || zAngle > 300 && zAngle < 340) //GREAT
        {
            Debug.Log("GREAT");
            boost = boostParameters.greatBoost;
        }

        else //BAD
        {
            Debug.Log("BAD");
            boost = Mathf.Clamp(boostParameters.failBoost,0,boost);
        }

        Debug.Log(zAngle);

        //RESTORE EVERYTHING
        GameManager.Instance.RestoreTime();
        GameManager.Instance.player.currentSpeed = previousSpeed + boost;
        GameManager.Instance.camera.cameraMode = CameraMode.railSmoothMode;
        GameManager.Instance.camera.cameraState = CameraState.moving;
        trigger.enabled = false;
        l_cooldownTime = cooldownTime;

        coroutineStarted = false;
    }

    //ENTER
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !coroutineStarted)
        {
            GameManager.Instance.SlowTime(slowTimeFactor);
            GameManager.Instance.camera.cameraMode = CameraMode.skillCheckMode;
            GameManager.Instance.camera.cameraState = CameraState.ring_skillcheck;

            previousSpeed = GameManager.Instance.player.currentSpeed;
            ringDistance = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

            GameManager.Instance.player.currentSpeed = ringDistance / skillcheckDuration;

            StartCoroutine("SkillCheck");
        }
    }

    //EXIT
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //RESTORE EVERYTHING
            GameManager.Instance.RestoreTime();
            GameManager.Instance.player.currentSpeed = previousSpeed;
            GameManager.Instance.camera.cameraMode = CameraMode.railSmoothMode;
            GameManager.Instance.camera.cameraState = CameraState.moving;

            trigger.enabled = false;
            l_cooldownTime = cooldownTime;
            StopCoroutine("SkillCheck");
        }
    }
}
