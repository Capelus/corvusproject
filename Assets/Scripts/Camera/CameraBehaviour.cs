using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum CameraState { idle, moving, braking, low_nitro, mid_nitro, high_nitro, ring_skillcheck }
public enum CameraMode { railMode, followMode, railSmoothMode, railSmoothModeUP, railSmoothModeInverted, railSmoothModeLookAt, skillCheckMode }

public class CameraBehaviour : MonoBehaviour
{
    //REFERENCES
    public CameraState cameraState;
    public CameraMode cameraMode;

    PlayerBehaviour player;
    Camera cam;
    public Volume postpro;
    Vignette _Vignette;
    float t = 0;

    //CORE PARAMETERS
    Vector3 cameraPos;
    float distanceToTarget;
    float desiredDistanceToTarget;
    float desiredfieldOfView;
    float desiredShakeAmount;
    float desiredVignetteIntensity = 0.2f;
    public float damp = 1.6f;

    //------------------------------------ EXTRA SETTINGS
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class Settings
    {
        public float sightBeyond = 20;
        public float lookAtSpeed = 6;
        public bool smooth2DFollow;
        public bool tiltWithTrack;
    }

    public Settings cameraSettings;
    //---------------------------------------------------

    //OTHER
    float hOffset = 0;
    float vOffset = 0;

    void Start()
    {
        //REFERENCES
        player = GameManager.Instance.player;
        GameManager.Instance.playerCamera = this;
        cam = GetComponent<Camera>();

        postpro.profile.TryGet<Vignette>(out _Vignette);
    }

    void Update()
    {
        //CAMERAS UPDATE
        t += Time.unscaledDeltaTime;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredfieldOfView, t); //FOV
        distanceToTarget = Mathf.Lerp(distanceToTarget, desiredDistanceToTarget, t); //DISTANCE TO SPACESHIP

        transform.localPosition += Random.insideUnitSphere * desiredShakeAmount; //CAMERA SHAKE    

        float vignetteIntensity = Mathf.Lerp(_Vignette.intensity.value, desiredVignetteIntensity, t); //VIGNETTE
        _Vignette.intensity.value = vignetteIntensity;

        //STATE MACHINE
        switch (cameraState)
        {
            case CameraState.idle:
                desiredDistanceToTarget = 6;
                desiredfieldOfView = 75;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0;
                break;

            case CameraState.moving:
                desiredDistanceToTarget = 6;
                desiredfieldOfView = 80;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0;
                break;

            case CameraState.braking:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 75;
                desiredVignetteIntensity = 0.2f;
                break;

            case CameraState.low_nitro:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 95;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.002f;
                break;

            case CameraState.mid_nitro:
                desiredDistanceToTarget = 2;
                desiredfieldOfView = 120;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.005f;
                break;

            case CameraState.high_nitro:
                desiredDistanceToTarget = 1;
                desiredfieldOfView = 150;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.01f;
                break;

            case CameraState.ring_skillcheck:
                desiredVignetteIntensity = 0.8f;
                break;
        }

        //CAMERA MODE
        switch (cameraMode)
        {
            case CameraMode.railMode:

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);
                cam.transform.position = cameraPos;
                cam.transform.forward = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled) - cam.transform.position;
                cam.transform.LookAt(TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled));             
                
                break;

            case CameraMode.railSmoothMode:

                t += damp * Time.deltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 2, t);
                vOffset = Mathf.Lerp(vOffset, player.verticalMove / 2, t);

                cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                transform.forward = Vector3.Lerp(transform.forward, (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized,t);

                break;

            case CameraMode.railSmoothModeUP:

                t += damp * Time.deltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 1.5f, t);
                vOffset = Mathf.Lerp(vOffset, (player.verticalMove + 2f) / 1.5f, t);

                if (cameraSettings.smooth2DFollow)
                    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                else cam.transform.position = cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos, t);

                transform.forward = Vector3.Lerp(transform.forward, (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized, t);

                break;

            case CameraMode.railSmoothModeInverted:

                t += damp * Time.deltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, -player.horizontalMove / 10, t);
                vOffset = Mathf.Lerp(vOffset, -player.verticalMove / 10, t);

                //cam.transform.position = cameraPos + transform.right * hOffset + cam.transform.up * vOffset;

                if (cameraSettings.smooth2DFollow)
                    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                else cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos, t);

                Vector3 lookAt = (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized;
                lookAt += (transform.right * -hOffset / 3) + (transform.up * -vOffset / 3);

                transform.forward = Vector3.Lerp(transform.forward, lookAt, t);

                break;

            case CameraMode.railSmoothModeLookAt:

                t += damp * Time.deltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 10f, t);
                vOffset = Mathf.Lerp(vOffset, (player.verticalMove) / 10f, t);

                if(cameraSettings.smooth2DFollow)
                    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                else cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos, t);

                Vector3 lookAtUP = (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized;
                lookAtUP += (transform.right * +hOffset / 5) + (transform.up * +vOffset / 5);

                transform.forward = Vector3.Lerp(transform.forward, lookAtUP, t);

                break;

            case CameraMode.skillCheckMode:

                t += damp * Time.unscaledDeltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, -3, t);
                vOffset = Mathf.Lerp(vOffset, -2, t);

                cam.transform.position = cameraPos + transform.right * hOffset + cam.transform.up * vOffset;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position), t);
                transform.LookAt(player.transform.position);

                break;
        }
    }

    private void LateUpdate()
    {
        if (cameraSettings.tiltWithTrack)
        {
            Quaternion rot = new Quaternion(transform.rotation.x, transform.rotation.y, player.transform.rotation.z, transform.rotation.w);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 20 * Time.deltaTime);
        }
    }

    public void ChangeState(CameraState state)
    {
        t = 0;
        cameraState = state;
    }
}
