using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParameters : MonoBehaviour
{
    [HideInInspector] public float initialDistanceOnPath;
    [HideInInspector] public int direction = 1;
    [HideInInspector] public float initialSpeed = 0;

    private void Awake()
    {
        //SET INITIAL DISTANCE ON PATH
        initialDistanceOnPath = TrackManager.Instance.GetClosestDistanceOnPath(transform.position);

        //DETECT DIRECTION
        Vector3 dir = TrackManager.Instance.GetDirectionAtDistance(initialDistanceOnPath);
        if (Vector3.Angle(transform.forward, dir) > 150)
            direction = -1;
    }
}
