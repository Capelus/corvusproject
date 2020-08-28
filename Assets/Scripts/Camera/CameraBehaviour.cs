using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum CameraState { idle, moving, low_nitro, mid_nitro, high_nitro }
public enum CameraMode { railMode, followMode, railSmoothMode }

public class CameraBehaviour : MonoBehaviour
{
    //REFERENCES
    public CameraState cameraState;
    public CameraMode cameraMode;

    Camera cam;
    PlayerBehaviour player;

    float t = 0;

    //CORE PARAMETERS
    float distanceToTarget;
    float desiredDistanceToTarget;
    float desiredfieldOfView;
    float desiredShakeAmount;

    //------------------------------------ EXTRA SETTINGS
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class Settings
    {
        public float sightBeyond = 20;
        public float lookAtSpeed = 6;
    }

    public Settings cameraSettings;
    //---------------------------------------------------

    //OTHER
    float hOffset = 0;
    float vOffset = 0;

    void Start()
    {
        //REFERENCES
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();

        cam = GetComponent<Camera>();
    }

    void Update()
    {
        //CAMERAS UPDATE

        //transform.forward = player.forwardDirection; //FORWARD

        t += 2 * Time.deltaTime;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredfieldOfView, t); //FOV
        distanceToTarget = Mathf.Lerp(distanceToTarget, desiredDistanceToTarget, t); //DISTANCE TO SPACESHIP

        transform.localPosition += Random.insideUnitSphere * desiredShakeAmount; //CAMERA SHAKE
        

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
                cam.transform.position = cameraPosition;
                cam.transform.forward = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled) - cam.transform.position;
                cam.transform.LookAt(TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled));             
                
                break;

            case CameraMode.railSmoothMode:

                t += 1.6f * Time.deltaTime;

                Vector3 cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 2, t);
                vOffset = Mathf.Lerp(vOffset, player.verticalMove / 2, t);

                cam.transform.position = cameraPos + transform.right * hOffset + cam.transform.up * vOffset;
                transform.forward = (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized;

                break;
        }
    }

    public void ChangeState(CameraState state)
    {
        t = 0;
        cameraState = state;
    }
}
