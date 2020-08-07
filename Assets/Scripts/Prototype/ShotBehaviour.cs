using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : MonoBehaviour
{
    public float speed = 80;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
