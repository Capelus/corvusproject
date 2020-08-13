using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState { idle, moving, low_nitro, mid_nitro, high_nitro}
public enum CameraMode { railMode, followMode}

public class CameraBehaviourD : MonoBehaviour
{
    //REFERENCES
    public CameraState cameraState;
    public CameraMode cameraMode;

    Camera cam;
    PlayerMovement player;

    float t = 0;

    //CORE PARAMETERS
    float distanceToTarget;
    float desiredDistanceToTarget;
    float desiredfieldOfView;

    //------------------------------------ EXTRA SETTINGS
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class ExtraSettings
    {        
    }
    //public ExtraSettings extraSettings;
    //---------------------------------------------------

    void Start()
    {
        //REFERENCES
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void LateUpdate()
    {
        //CAMERA UPDATE
        transform.forward = player.forwardDirection;

        t += 2 * Time.deltaTime;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredfieldOfView, t);
        distanceToTarget = Mathf.Lerp(distanceToTarget, desiredDistanceToTarget, t);

        //STATE MACHINE
        switch (cameraState)
        {
            case CameraState.idle:
                desiredDistanceToTarget = 5;
                desiredfieldOfView = 75;
                break;

            case CameraState.moving:
                desiredDistanceToTarget = 6;
                desiredfieldOfView = 80;
                break;

            case CameraState.low_nitro:
                desiredDistanceToTarget = 5;
                desiredfieldOfView = 95;
                break;

            case CameraState.mid_nitro:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 110;
                break;

            case CameraState.high_nitro:
                desiredDistanceToTarget = 3;
                desiredfieldOfView = 140;
                break;
        }

        //CAMERA MODE
        switch (cameraMode)
        {
            case CameraMode.railMode:
                Vector3 cameraPosition = TrackManager.tm.GetPositionAtDistance(player.distanceTravelled-distanceToTarget);
                transform.position = cameraPosition;
                transform.forward = TrackManager.tm.GetPositionAtDistance(player.distanceTravelled) - transform.position;

                break;

            case CameraMode.followMode:
                transform.position = player.transform.position - transform.forward * distanceToTarget;
                transform.LookAt(player.transform.position);
                transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                break;
        }
    }

    public void ChangeState(CameraState state)
    {
        t = 0;
        cameraState = state;
    }
}
