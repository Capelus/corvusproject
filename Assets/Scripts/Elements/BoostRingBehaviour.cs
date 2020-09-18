using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRingBehaviour : MonoBehaviour
{
    //REFERENCES
    GameObject ring;
    CapsuleCollider trigger;

    GameObject skillcheck;
    public float ringRotationSpeed = 2;
    public float skillcheckDuration = 2;
    public float slowTimeFactor = 4;
    public float cooldownTime = 2;
    float l_cooldownTime = 0;

    //BOOST PARAMETERS
    [System.Serializable]
    public class BoostParameters
    {
        public float failBoost;
        public float failBoostTime = 0.2f;
        public float greatBoost;
        public float greatBoostTime = 1;
        public float perfectBoost;
        public float perfectBoostTime = 1.5f;
    }
    public BoostParameters boostParameters;

    float previousSpeed;
    bool coroutineStarted = false;

    private void Start()
    {
        ring = transform.GetChild(0).gameObject;
        trigger = GetComponent<CapsuleCollider>();
        skillcheck = UIManager.Instance.UI.skillcheck;
    }

    private void Update()
    {
        //ROTATE
        ring.transform.Rotate(0, ringRotationSpeed * Time.deltaTime, 0);

        l_cooldownTime -= Time.unscaledDeltaTime;
        if (l_cooldownTime < 0 && !trigger.enabled)
            trigger.enabled = true;
    }

    //SKILLCHECK COROUTINE -- UNUSED
    IEnumerator SkillCheck()
    {
        coroutineStarted = true;

        //WAIT FOR ROLL
        while (!GameManager.Instance.playerInput.roll)
        {
            yield return null;
        }

        //RESTORE PREVIOUS SPEED
        GameManager.Instance.player.currentSpeed = previousSpeed;

        //BOOST
        switch (skillcheck.GetComponent<SkillcheckBehaviour>().skillcheckState)
        {
            case 0: //BAD
                Debug.Log("BAD");
                GameManager.Instance.player.Boost(boostParameters.failBoostTime, boostParameters.failBoost, 0, CameraState.moving);
                GameManager.Instance.player.animator.SetBool("Impact", true);
                break;

            case 1: //GOOD
                Debug.Log("GOOD");
                GameManager.Instance.player.Boost(boostParameters.greatBoostTime, boostParameters.greatBoost, 20, CameraState.low_nitro);
                break;

            case 2: //PERFECT
                Debug.Log("PERFECT");
                GameManager.Instance.player.Boost(boostParameters.perfectBoostTime, boostParameters.perfectBoost, 30, CameraState.mid_nitro);
                break;
        }

        //RESTORE EVERYTHING
        UIManager.Instance.UI.skillcheck.SetActive(false);
        GameManager.Instance.RestoreTime();
        GameManager.Instance.playerCamera.cameraMode = CameraMode.railSmoothModeUP;
        GameManager.Instance.playerCamera.ChangeState(CameraState.moving);
        trigger.enabled = false;
        l_cooldownTime = cooldownTime;

        coroutineStarted = false;
    }

    //ENTER
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !coroutineStarted)
        {
            GameManager.Instance.player.Boost(boostParameters.perfectBoostTime, boostParameters.perfectBoost, 30, CameraState.mid_nitro);
            l_cooldownTime = cooldownTime;

            //UIManager.Instance.UI.skillcheck.SetActive(true);
            //GameManager.Instance.SlowTime(slowTimeFactor);

            //previousSpeed = GameManager.Instance.player.currentSpeed;
            //float ringDistance = Vector3.Distance(transform.position, GameManager.Instance.player.transform.position);

            //GameManager.Instance.player.currentSpeed = ringDistance / skillcheckDuration;

            //GameManager.Instance.playerCamera.ChangeState(CameraState.ring_skillcheck);

            //StartCoroutine("SkillCheck");
        }
    }

    //EXIT
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //PUNISH
    //        GameManager.Instance.player.currentSpeed = Mathf.Clamp(previousSpeed + boostParameters.failBoost, 0, previousSpeed);
    //        GameManager.Instance.player.animator.SetBool("Impact", true);

    //        //RESTORE EVERYTHING
    //        UIManager.Instance.UI.skillcheck.SetActive(false);
    //        GameManager.Instance.RestoreTime();
    //        GameManager.Instance.playerCamera.cameraMode = CameraMode.railSmoothModeUP;
    //        GameManager.Instance.playerCamera.ChangeState(CameraState.moving);

    //        trigger.enabled = false;
    //        l_cooldownTime = cooldownTime;
    //        StopCoroutine("SkillCheck");
    //    }
    //}
}
