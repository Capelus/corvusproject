using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunRingBehaviour : MonoBehaviour
{
    //REFERENCES
    CapsuleCollider trigger;

    //PARAMETERS
    public float stunDuration = 2;

    //ROTATION
    public float ringRotationSpeed = 2;

    //COOLDOWN
    public float cooldownTime = 2;
    float l_cooldownTime = 0;

    private void Start()
    {
        trigger = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        //ROTATE
        transform.Rotate(0, 0, ringRotationSpeed * Time.deltaTime);

        l_cooldownTime -= Time.deltaTime;
        if (l_cooldownTime < 0)
            trigger.enabled = true;

        else trigger.enabled = false;
    }

    //ENTER
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.player.Stun(stunDuration);
            l_cooldownTime = cooldownTime;
        }

        if (other.CompareTag("Racer"))
        {
            //other.GetComponent<AIBehaviour>().
        }
    }
}
