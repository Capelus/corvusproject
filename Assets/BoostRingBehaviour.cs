using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostRingBehaviour : MonoBehaviour
{
    //REFERENCES
    GameObject ring;

    public float rotationSpeed = 50;
    public float skillcheckRotationSpeed = 50;
    public float slowTimeFactor = 4;

    float zAngle;

    private void Start()
    {
        ring = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        //ROTATE
        ring.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    //SKILLCHECK COROUTINE
    IEnumerator SkillCheck()
    {
        while (!PlayerInput.roll)
        {
            ring.transform.Rotate(0, 0, skillcheckRotationSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        zAngle = ring.transform.rotation.normalized.eulerAngles.z;

        //GIVE BOOST
        if (zAngle > 100 && zAngle < 120 || zAngle > 220 && zAngle < 240 || zAngle > 340 && zAngle < 360) //PERFECT
        {
            Debug.Log("PERFECT");
        }

        else if (zAngle > 60 && zAngle < 100 || zAngle > 180 && zAngle < 220 || zAngle > 300 && zAngle < 340) //GREAT
        {
            Debug.Log("GREAT");
        }

        else //BAD
        {
            Debug.Log("BAD");
        }

        Debug.Log(zAngle);

        //RESTORE EVERYTHING
        GameManager.Instance.RestoreTime();
    }

    //ENTER
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SlowTime(slowTimeFactor);
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
            StopCoroutine("SkillCheck");
        }
    }
}
