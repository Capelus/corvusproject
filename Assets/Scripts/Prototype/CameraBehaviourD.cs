using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState { idle, moving, low_nitro, mid_nitro, high_nitro}
public enum CameraMode { staticMode, dynamicMode}

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
        public bool staticMode = true;
    }
    public ExtraSettings extraSettings;
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
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 75;
                break;

            case CameraState.moving:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 80;
                break;

            case CameraState.low_nitro:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 95;
                break;

            case CameraState.mid_nitro:
                desiredDistanceToTarget = 3;
                desiredfieldOfView = 110;
                break;

            case CameraState.high_nitro:
                desiredDistanceToTarget = 2;
                desiredfieldOfView = 140;
                break;
        }

        //CAMERA MODE
        switch (cameraMode)
        {
            case CameraMode.staticMode:
                transform.position = new Vector3(player.transform.position.x + distanceToTarget, transform.position.y, transform.position.z);
                break;

            case CameraMode.dynamicMode:
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
