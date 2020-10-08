using UnityEngine;
using UnityEngine.Rendering;

public enum CameraMode { railSmoothMode, railSmoothModeInverted, railSmoothModeLookAt }
public enum CameraState { idle, moving, braking, boost, superboost }

public class CameraBehaviour : MonoBehaviour
{
    // REFERENCES
    PlayerBehaviour player;
    Camera cam;

    // ENUMERABLES
    public CameraState cameraState;

    // CORE PARAMETERS
    Vector3 cameraPos;
    float distanceToTarget, fieldOfView, shakeAmount;
    public float stateTransitionTime = 1;
    float l_stateTransitionTime = 0;

    // TIMESTEPS
    float t = 0, damp = 0.4f;

    // CAMERA STATES
    [System.Serializable]
    public class StateParameters
    {
        // DISTANCE TO TARGET
        [Tooltip("This curve determines the rate at which the camera gets closer or farther (over speed).")]
        public AnimationCurve distanceToTargetCurve;

        // FIELD OF VIEW
        [Tooltip("This curve determines the rate at which the camera changes it's FOV (over speed).")]
        public AnimationCurve fieldOfViewCurve;

        // SHAKE AMOUNT
        [Tooltip("The maximum shake amount the camera can take.")]
        public float maxShakeAmount = 0.001f;
    }

    [Header("CAMERA STATE PARAMETERS")]
    public StateParameters idleParameters;
    public StateParameters movingParameters;

    StateParameters currentParameters;

    [Header("CAMERA SETTINGS")]
    [Tooltip("The minimum distance the camera can get to it's target.)")]
    public float minDistanceToTarget = 5;
    float l_minDistanceToTarget;
    [Tooltip("The maximum distance the camera can get from it's target.")]
    public float maxDistanceToTarget = 8;
    float l_maxDistanceToTarget;

    [Tooltip("The minimum field of view the camera can reach.")]
    public float minFieldOfView = 75;
    float l_minFieldOfView;
    [Tooltip("The maximum field of view the camera can reach.")]
    public float maxFieldOfView = 120;
    float l_maxFieldOfView;

    // LOCAL
    float hOffset = 0; // This value stores the horizontal offset relative to the player's horizontal position on screen.
    float vOffset = 0; // This value stores the vertical offset relative to the player's vertical position on screen.

    // SIGHT BEYOND PLAYER'S SPACESHIP
    [Header("OTHER")]
    public float sightBeyond = 20;

    // THE POSTPROCESSING VOLUME IF ANY
    public Volume postpro;

    // EFFECTS
    float warpSpeed = 0;
    bool nebulaActive;
    float nebulaDissolve, nebulaSpeed;

    void Start()
    {
        // REFERENCES
        GameManager.Instance.playerCamera = this;
        player = GameManager.Instance.player;
        cam = GetComponent<Camera>();

        // PARAMETERS
        l_maxDistanceToTarget = maxDistanceToTarget;
        l_minDistanceToTarget = minDistanceToTarget;
        l_maxFieldOfView = maxFieldOfView;
        l_minFieldOfView = minFieldOfView;

        //OTHER
        currentParameters = idleParameters;
        stateTransitionTime *= 100;
    }

    void Update()
    {
        // TIMESTEPS
        t += damp * Time.deltaTime;
        l_stateTransitionTime += Time.deltaTime;

        // DISTANCE
        float desiredDistance = currentParameters.distanceToTargetCurve.Evaluate(player.currentSpeed / player.l_maxSpeed) * l_maxDistanceToTarget; 
        distanceToTarget = Mathf.Lerp(distanceToTarget, desiredDistance, (l_stateTransitionTime / stateTransitionTime));
        distanceToTarget = Mathf.Clamp(distanceToTarget, l_minDistanceToTarget, l_maxDistanceToTarget);

        // FOV
        float desiredFOV = currentParameters.fieldOfViewCurve.Evaluate(player.currentSpeed / player.l_maxSpeed) * l_maxFieldOfView;
        fieldOfView = Mathf.Lerp(fieldOfView, desiredFOV, (l_stateTransitionTime / stateTransitionTime));
        fieldOfView = Mathf.Clamp(fieldOfView, l_minFieldOfView, l_maxFieldOfView);
        cam.fieldOfView = fieldOfView;

        // SHAKE
        shakeAmount = (player.currentSpeed / player.l_maxSpeed) * currentParameters.maxShakeAmount;
        transform.localPosition += Random.insideUnitSphere * shakeAmount;

        // GET POSITION
        cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

        // CALCULATE 2D OFFSETS
        hOffset = Mathf.Lerp(hOffset, -player.horizontalMove / 4, t);
        vOffset = Mathf.Lerp(vOffset, -player.verticalMove / 4, t);

        // CALULATE POSITION
        transform.position = Vector3.Lerp(transform.position, cameraPos + transform.right * hOffset + transform.up * vOffset, t);
               
        // CALULATE ROTATION
        transform.LookAt(TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + sightBeyond), player.transform.up);
    }

    public void ChangeState(CameraState state)
    {
        if (cameraState != state)
        {
            switch (state)
            {
                case CameraState.idle:
                    currentParameters = idleParameters;
                    l_maxFieldOfView = maxFieldOfView;
                    break;

                case CameraState.moving:
                    currentParameters = movingParameters;
                    l_maxFieldOfView = maxFieldOfView;
                    break;

                case CameraState.braking:
                    currentParameters = idleParameters;
                    l_maxFieldOfView = maxFieldOfView - 10;
                    break;

                case CameraState.boost:
                    currentParameters = movingParameters;
                    l_maxFieldOfView = maxFieldOfView + 30;
                    break;

                case CameraState.superboost:
                    currentParameters = movingParameters;
                    l_maxFieldOfView = maxFieldOfView + 50;
                    break;
            }

            cameraState = state;
            l_stateTransitionTime = 0;
        }
    }
}