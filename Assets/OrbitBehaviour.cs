using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitBehaviour : MonoBehaviour
{
    public Transform orbitTarget;

    public float orbitSpeed = 10;
    float orbitDistance;

    private void Start()
    {
        orbitDistance = Vector3.Distance(transform.position, orbitTarget.position);
    }

    void Update()
    {
        transform.RotateAround(orbitTarget.position, transform.forward, orbitSpeed * Time.deltaTime);
        var desiredPosition = (transform.position - orbitTarget.position).normalized * orbitDistance + orbitTarget.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * orbitSpeed);
    }
}
