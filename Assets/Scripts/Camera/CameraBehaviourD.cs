using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CameraState { idle, moving, low_nitro, mid_nitro, high_nitro }
public enum CameraMode { railMode, followMode, railSmoothMode }

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
    float desiredShakeAmount;

    //------------------------------------ EXTRA SETTINGS
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class ExtraSettings
    {
    }
    //public ExtraSettings extraSettings;
    //---------------------------------------------------

    //OTHER
    float hOffset = 0;
    float vOffset = 0;

    void Start()
    {
        //REFERENCES
        cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
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

                //CAMERA SHAKE
                desiredShakeAmount = 0;
                break;

            case CameraState.moving:
                desiredDistanceToTarget = 6;
                desiredfieldOfView = 80;

                //CAMERA SHAKE
                desiredShakeAmount = 0;
                break;

            case CameraState.low_nitro:
                desiredDistanceToTarget = 5;
                desiredfieldOfView = 95;

                //CAMERA SHAKE
                desiredShakeAmount = 0.02f;
                break;

            case CameraState.mid_nitro:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 110;

                //CAMERA SHAKE
                desiredShakeAmount = 0.05f;
                break;

            case CameraState.high_nitro:
                desiredDistanceToTarget = 3;
                desiredfieldOfView = 140;

                //CAMERA SHAKE
                desiredShakeAmount = 0.1f;
                break;
        }

        //CAMERA MODE
        switch (cameraMode)
        {
            case CameraMode.railMode:
                Vector3 cameraPosition = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);
                transform.position = cameraPosition;
                transform.forward = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled) - transform.position;
                transform.LookAt(TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled));
                break;

            case CameraMode.followMode:
                transform.position = player.transform.position - transform.forward * distanceToTarget;
                transform.LookAt(player.transform.position);
                transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                break;

            case CameraMode.railSmoothMode:
                t += 1.6f * Time.deltaTime;

                Vector3 cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 2, t);
                vOffset = Mathf.Lerp(vOffset, player.verticalMove / 2, t);

                transform.position = cameraPos + transform.right * hOffset + transform.up * vOffset;
                transform.forward = player.transform.forward;
                break;
        }

        //CAMERA SHAKE
        transform.localPosition += Random.insideUnitSphere * desiredShakeAmount;
    }

    public void ChangeState(CameraState state)
    {
        t = 0;
        cameraState = state;
    }
}
