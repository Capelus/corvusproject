using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public enum CameraMode { railSmoothMode, railSmoothModeInverted, railSmoothModeLookAt }
public enum CameraState { idle, moving, braking, boost, superboost, startingSequence }

public class CameraBehaviour : MonoBehaviour
{
    //REFERENCES
    public CameraMode cameraMode = CameraMode.railSmoothModeInverted;
    public CameraState cameraState;
    public Animator camAnimator;

    PlayerBehaviour player;
    Camera cam;
    Vignette _Vignette;

    //CORE PARAMETERS
    Vector3 cameraPos;
    float distanceToTarget, desiredDistanceToTarget;
    float desiredfieldOfView;
    float desiredShakeAmount;
    float desiredVignetteIntensity;
    
    //TIMESTEPS
    float d = 0, t = 0, stateDamp = 0.6f, modeDamp = 1f;

    //CAMERA STATES
    [System.Serializable]
    public class CameraProfile
    {
        [Header("PARAMETERS")]
        //PARAMETERS
        public float distanceToTarget = 5;
        public float fieldOfView = 75;
        public float shakeAmount = 0.001f;
        public float vignetteIntensity = 0.2f;

        [Header("EFFECTS")]
        //EFFECTS
        public float warpSpeed = 0;
        public bool nebulaActive;
        public float nebulaDissolve, nebulaSpeed;
    }

    [Header("CAMERA STATES")]
    public CameraProfile idle;
    public CameraProfile moving;
    public CameraProfile braking;
    public CameraProfile boost;
    public CameraProfile superboost;

    //LOCAL
    float hOffset = 0;
    float vOffset = 0;

    //OTHER
    [Header("OTHER")]
    //SIGHT BEYOND PLAYER'S SPACESHIP
    public float sightBeyond = 20;
    public Volume postpro;

    void Start()
    {
        //REFERENCES
        GameManager.Instance.playerCamera = this;
        player = GameManager.Instance.player;
        cam = GetComponent<Camera>();
        postpro.profile.TryGet<Vignette>(out _Vignette);
    }

    void Update()
    {

        if (UIManager.Instance.UIW.countDown.enabled && camAnimator.enabled)
        {
            camAnimator.enabled = false;
            
        }
        //TIMESTEPS
        d += modeDamp * Time.unscaledDeltaTime;
        t += stateDamp * Time.deltaTime;

        //FOV
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredfieldOfView, d);

        //DISTANCE TO TARGET
        distanceToTarget = Mathf.Lerp(distanceToTarget, desiredDistanceToTarget, d);

        //SHAKE
        transform.localPosition += Random.insideUnitSphere * desiredShakeAmount; 

        //VIGNETTE INTENISTY
        float vignetteIntensity = Mathf.Lerp(_Vignette.intensity.value, desiredVignetteIntensity, d); //VIGNETTE
        _Vignette.intensity.value = vignetteIntensity;

        //CAMERA MODES
        switch (cameraMode)
        {
            case CameraMode.railSmoothMode:
                //GET POSITION
                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                //CALCULATE 2D OFFSETS
                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 2, t);
                vOffset = Mathf.Lerp(vOffset, player.verticalMove / 2, t);

                //CALULATE POSITION
                cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                //CALULATE ROTATION
                transform.forward = Vector3.Lerp(transform.forward, (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + sightBeyond) - transform.position).normalized,t);
                break;

            case CameraMode.railSmoothModeInverted:
                //GET POSITION
                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                //CALCULATE 2D OFFSETS
                hOffset = Mathf.Lerp(hOffset, -player.horizontalMove / 12, t);
                vOffset = Mathf.Lerp(vOffset, -player.verticalMove / 12, t);

                //CALULATE POSITION
                cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);

                //CALULATE ROTATION
                Vector3 lookAt = (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + sightBeyond) - transform.position).normalized;
                lookAt += (transform.right * -hOffset / 3) + (transform.up * -vOffset / 4);
                transform.forward = Vector3.Lerp(transform.forward, lookAt, t);
                break;

            case CameraMode.railSmoothModeLookAt:
                //GET POSITION
                cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

                //CALCULATE 2D OFFSETS
                hOffset = Mathf.Lerp(hOffset, player.horizontalMove / 10f, t);
                vOffset = Mathf.Lerp(vOffset, (player.verticalMove) / 10f, t);

                //CALULATE POSITION
                cam.transform.position = Vector3.Lerp(cam.transform.position, cameraPos + transform.right * hOffset + cam.transform.up * vOffset, t);
                
                //CALULATE ROTATION
                Vector3 lookAtUP = (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + sightBeyond) - transform.position).normalized;
                lookAtUP += (transform.right * +hOffset / 5) + (transform.up * +vOffset / 5);
                transform.forward = Vector3.Lerp(transform.forward, lookAtUP, t);
                break;
        }

        switch (cameraState)
        {
            case CameraState.idle:
                //SET VALUES FROM CURRENT STATE
                desiredDistanceToTarget = idle.distanceToTarget;
                desiredfieldOfView = idle.fieldOfView;
                desiredShakeAmount = idle.shakeAmount;
                desiredVignetteIntensity = idle.shakeAmount;

                //CHANGE EFFECTS
                EffectsManager.Instance.effects.warpSpeed = idle.warpSpeed;
                EffectsManager.Instance.effects.nebulaActive = idle.nebulaActive;
                EffectsManager.Instance.effects.nebulaSpeed = idle.nebulaSpeed;
                EffectsManager.Instance.effects.nebulaDissolve = idle.nebulaDissolve;
                break;

            case CameraState.moving:
                //SET VALUES FROM CURRENT STATE
                desiredDistanceToTarget = moving.distanceToTarget;
                desiredfieldOfView = moving.fieldOfView;
                desiredShakeAmount = moving.shakeAmount;
                desiredVignetteIntensity = moving.shakeAmount;

                //CHANGE EFFECTS
                EffectsManager.Instance.effects.warpSpeed = moving.warpSpeed;
                EffectsManager.Instance.effects.nebulaActive = moving.nebulaActive;
                EffectsManager.Instance.effects.nebulaSpeed = moving.nebulaSpeed;
                EffectsManager.Instance.effects.nebulaDissolve = moving.nebulaDissolve;
                break;

            case CameraState.braking:
                //SET VALUES FROM CURRENT STATE
                desiredDistanceToTarget = braking.distanceToTarget;
                desiredfieldOfView = braking.fieldOfView;
                desiredShakeAmount = braking.shakeAmount;
                desiredVignetteIntensity = braking.shakeAmount;

                //CHANGE EFFECTS
                EffectsManager.Instance.effects.warpSpeed = braking.warpSpeed;
                EffectsManager.Instance.effects.nebulaActive = braking.nebulaActive;
                EffectsManager.Instance.effects.nebulaSpeed = braking.nebulaSpeed;
                EffectsManager.Instance.effects.nebulaDissolve = braking.nebulaDissolve;
                break;

            case CameraState.boost:
                //SET VALUES FROM CURRENT STATE
                desiredDistanceToTarget = boost.distanceToTarget;
                desiredfieldOfView = boost.fieldOfView;
                desiredShakeAmount = boost.shakeAmount;
                desiredVignetteIntensity = boost.shakeAmount;

                //CHANGE EFFECTS
                EffectsManager.Instance.effects.warpSpeed = boost.warpSpeed;
                EffectsManager.Instance.effects.nebulaActive = boost.nebulaActive;
                EffectsManager.Instance.effects.nebulaSpeed = boost.nebulaSpeed;
                EffectsManager.Instance.effects.nebulaDissolve = boost.nebulaDissolve;
                break;

            case CameraState.superboost:
                //SET VALUES FROM CURRENT STATE
                desiredDistanceToTarget = superboost.distanceToTarget;
                desiredfieldOfView = superboost.fieldOfView;
                desiredShakeAmount = superboost.shakeAmount;
                desiredVignetteIntensity = superboost.shakeAmount;

                //CHANGE EFFECTS
                EffectsManager.Instance.effects.warpSpeed = superboost.warpSpeed;
                EffectsManager.Instance.effects.nebulaActive = superboost.nebulaActive;
                EffectsManager.Instance.effects.nebulaSpeed = superboost.nebulaSpeed;
                EffectsManager.Instance.effects.nebulaDissolve = superboost.nebulaDissolve;
                break;
        }
    }

    public void ChangeState(CameraState state)
    {
        t = 0;
        d = 0;
        cameraState = state;       
    }
}
