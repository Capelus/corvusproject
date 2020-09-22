using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum CameraState { idle, moving, braking, low_nitro, mid_nitro, high_nitro, boost, superboost, ring_skillcheck }
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
    float d = 0;
    float t = 0;

    //CORE PARAMETERS
    Vector3 cameraPos;
    float distanceToTarget;
    float desiredDistanceToTarget;
    float desiredfieldOfView;
    float desiredShakeAmount;
    float desiredVignetteIntensity = 0.2f;
    public float stateDamp = 1.6f;
    public float modeDamp = 1f;

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
        d += modeDamp * Time.unscaledDeltaTime;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredfieldOfView, d); //FOV
        distanceToTarget = Mathf.Lerp(distanceToTarget, desiredDistanceToTarget, d); //DISTANCE TO SPACESHIP

        transform.localPosition += Random.insideUnitSphere * desiredShakeAmount; //CAMERA SHAKE    

        float vignetteIntensity = Mathf.Lerp(_Vignette.intensity.value, desiredVignetteIntensity, d); //VIGNETTE
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

                //EFFECTS
                EffectsManager.Instance.effects.warpSpeed = 0;
                EffectsManager.Instance.effects.nebulaActive = false;
                break;

            case CameraState.moving:
                desiredDistanceToTarget = 6;
                desiredfieldOfView = 80;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0;

                //EFFECTS
                EffectsManager.Instance.effects.warpSpeed = 0;
                EffectsManager.Instance.effects.nebulaActive = false;
                break;

            case CameraState.braking:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 75;
                desiredVignetteIntensity = 0.2f;

                //EFFECTS
                EffectsManager.Instance.effects.warpSpeed = 0;
                EffectsManager.Instance.effects.nebulaActive = false;
                break;

            case CameraState.low_nitro:
                desiredDistanceToTarget = 4;
                desiredfieldOfView = 95;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.002f;

                //PARTICLES
                EffectsManager.Instance.effects.warpSpeed = 0.3f;
                EffectsManager.Instance.effects.nebulaActive = false;
                break;

            case CameraState.mid_nitro:
                desiredDistanceToTarget = 2;
                desiredfieldOfView = 120;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.005f;

                //EFFECTS
                EffectsManager.Instance.effects.warpSpeed = 0.6f;
                EffectsManager.Instance.effects.nebulaActive = true;
                EffectsManager.Instance.effects.nebulaDissolve = 1;
                EffectsManager.Instance.effects.nebulaSpeed = 0.3f;
                break;

            case CameraState.high_nitro:
                desiredDistanceToTarget = 1;
                desiredfieldOfView = 150;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.01f;

                //EFFECTS
                EffectsManager.Instance.effects.warpSpeed = 1f;
                EffectsManager.Instance.effects.nebulaActive = true;
                EffectsManager.Instance.effects.nebulaDissolve = 0.3f;
                EffectsManager.Instance.effects.nebulaSpeed = 1f;
                break;

            case CameraState.boost:
                desiredDistanceToTarget = 5;
                desiredfieldOfView = 100;
                desiredVignetteIntensity = 0.2f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.004f;

                //PARTICLES
                EffectsManager.Instance.effects.warpSpeed = 0.3f;
                EffectsManager.Instance.effects.nebulaActive = false;
                break;

            case CameraState.superboost:
                desiredDistanceToTarget = 3;
                desiredfieldOfView = 130;
                desiredVignetteIntensity = 0.3f;

                //CAMERA SHAKE
                desiredShakeAmount = 0.01f;

                //EFFECTS
                EffectsManager.Instance.effects.warpSpeed = 0.6f;
                EffectsManager.Instance.effects.nebulaActive = true;
                EffectsManager.Instance.effects.nebulaDissolve = 1;
                EffectsManager.Instance.effects.nebulaSpeed = 0.3f;
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

                t += stateDamp * Time.deltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 2, t);
                vOffset = Mathf.Lerp(vOffset, player.verticalMove / 2, t);

                cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                transform.forward = Vector3.Lerp(transform.forward, (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized,t);

                break;

            case CameraMode.railSmoothModeUP:

                t += stateDamp * Time.deltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 1.5f, t);
                vOffset = Mathf.Lerp(vOffset, (player.verticalMove + 2f) / 1.5f, t);

                if (cameraSettings.smooth2DFollow)
                    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                else cam.transform.position = cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos, t);

                transform.forward = Vector3.Lerp(transform.forward, (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized, t);

                break;

            case CameraMode.railSmoothModeInverted:

                t += stateDamp * Time.deltaTime;

                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                hOffset = Mathf.Lerp(hOffset, -player.horizontalMove / 12, t);
                vOffset = Mathf.Lerp(vOffset, -player.verticalMove / 12, t);

                if (cameraSettings.smooth2DFollow)
                    cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                else cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos, t);

                Vector3 lookAt = (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + cameraSettings.sightBeyond) - transform.position).normalized;
                lookAt += (transform.right * -hOffset / 3) + (transform.up * -vOffset / 4);

                transform.forward = Vector3.Lerp(transform.forward, lookAt, t);

                //transform.LookAt(lookAt);

                break;

            case CameraMode.railSmoothModeLookAt:

                t += stateDamp * Time.deltaTime;

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

                t += stateDamp * Time.unscaledDeltaTime;

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
            Quaternion rot = new Quaternion(transform.rotation.x, transform.rotation.y, TrackManager.Instance.GetRotationAtDistance(player.distanceTravelled).z, transform.rotation.w);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 20 * Time.deltaTime);
        }
    }

    public void ChangeState(CameraState state)
    {
        t = 0;
        d = 0;
        cameraState = state;
    }
}
