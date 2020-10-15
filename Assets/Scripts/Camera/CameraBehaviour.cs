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
    float t = 0, damp = 0.8f;

    // CAMERA STATES
    [System.Serializable]
    public class StateParameters
    {
        //LOOK AT SPEED
        [Tooltip("The speed at the camera will rotate to face it's target.)")]
        public float lookAtSpeed = 50;

        // DISTANCE TO TARGET
        [Tooltip("The minimum distance the camera can get to it's target.)")]
        public float minDistanceToTarget = 5;

        [Tooltip("The maximum distance the camera can get from it's target.")]
        public float maxDistanceToTarget = 8;

        [Tooltip("This curve determines the rate at which the camera gets closer or farther (over speed).")]
        public AnimationCurve distanceToTargetCurve;

        // FIELD OF VIEW
        [Tooltip("The minimum field of view the camera can reach.")]
        public float minFieldOfView = 75;

        [Tooltip("The maximum field of view the camera can reach.")]
        public float maxFieldOfView = 120;

        [Tooltip("This curve determines the rate at which the camera changes it's FOV (over speed).")]
        public AnimationCurve fieldOfViewCurve;

        // SHAKE AMOUNT
        [Tooltip("The maximum shake amount the camera can take.")]
        public float maxShakeAmount = 0.001f;
    }

    [Header("CAMERA STATE PARAMETERS")]
    public StateParameters movingParameters;
    public StateParameters boostParameters;
    StateParameters currentParameters;

    float hOffset = 0; // This value stores the horizontal offset relative to the player's horizontal position on screen.
    float vOffset = 0; // This value stores the vertical offset relative to the player's vertical position on screen.

    // SIGHT BEYOND PLAYER'S SPACESHIP
    [Header("SETTINGS")]
    [Tooltip("The distance to look at beyond the player.")]
    public float sightBeyond = 20;
    [Tooltip("Check this to invert the offset movements.")]
    public bool invertedOffsets;
    [Tooltip("The fraction (1/X) from the player's horizontal movement used to calculate the horizontal offset value.")]
    public float horizontalOffsetFraction = 4;
    [Tooltip("The fraction (1/X) from the player's vertical movement used to calculate the vertical offset value.")]
    public float verticalOffsetFraction = 3;

    // THE POSTPROCESSING VOLUME IF ANY
    //public Volume postpro;

    void Start()
    {
        // REFERENCES
        GameManager.Instance.playerCamera = this;
        player = GameManager.Instance.player;
        cam = GetComponent<Camera>();

        // PARAMETERS
        ChangeState(CameraState.moving);
    }

    void Update()
    {
        // TIMESTEPS
        t += damp * Time.deltaTime;
        l_stateTransitionTime += Time.deltaTime;
        l_stateTransitionTime = Mathf.Clamp(l_stateTransitionTime, 0, 1); // Clamp the value on a 0-1 range.

        // DISTANCE
        float desiredDistance = currentParameters.minDistanceToTarget + currentParameters.distanceToTargetCurve.Evaluate(player.currentSpeed / player.l_maxSpeed) * (currentParameters.maxDistanceToTarget - currentParameters.minDistanceToTarget); 
        distanceToTarget = Mathf.Lerp(distanceToTarget, desiredDistance, (l_stateTransitionTime / stateTransitionTime));

        // FOV
        float desiredFOV = currentParameters.minFieldOfView + currentParameters.fieldOfViewCurve.Evaluate(player.currentSpeed / player.l_maxSpeed) * (currentParameters.maxFieldOfView - currentParameters.minFieldOfView);
        fieldOfView = Mathf.Lerp(fieldOfView, desiredFOV, (l_stateTransitionTime / stateTransitionTime));
        cam.fieldOfView = fieldOfView;

        // SHAKE
        shakeAmount = (player.currentSpeed / player.l_maxSpeed) * currentParameters.maxShakeAmount;
        transform.localPosition += Random.insideUnitSphere * shakeAmount;

        // GET POSITION
        cameraPos = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled - distanceToTarget);

        // CALCULATE 2D OFFSETS
        if (invertedOffsets)
        {
            hOffset = Mathf.Lerp(hOffset, -player.horizontalMove / horizontalOffsetFraction, t);
            vOffset = Mathf.Lerp(vOffset, -player.verticalMove / verticalOffsetFraction, t);
        }

        else
        {
            hOffset = Mathf.Lerp(hOffset, -player.horizontalMove / horizontalOffsetFraction, t);
            vOffset = Mathf.Lerp(vOffset, player.verticalMove / verticalOffsetFraction, t);
        }

        // CALULATE POSITION
        transform.position = Vector3.Lerp(transform.position, cameraPos + transform.right * hOffset + transform.up * vOffset, t);

        // CALULATE ROTATION
        //transform.LookAt(TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + sightBeyond), player.transform.up); //OLD WAY
        Vector3 lookDirection = (TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + sightBeyond)) - transform.position;
        lookDirection = Quaternion.AngleAxis(-hOffset * 2, player.transform.up) * lookDirection;
        lookDirection = Quaternion.AngleAxis(-vOffset * 2, player.transform.right) * lookDirection;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection, player.transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, currentParameters.lookAtSpeed * Time.deltaTime);
    }

    public void ChangeState(CameraState state)
    {
        if (cameraState != state)
        {
            switch (state)
            {
                case CameraState.idle:
                    currentParameters = movingParameters;
                    break;

                case CameraState.moving:
                    currentParameters = movingParameters;
                    break;

                case CameraState.braking:
                    currentParameters = movingParameters;
                    break;

                case CameraState.boost:
                    currentParameters = boostParameters;
                    break;

                case CameraState.superboost:
                    currentParameters = boostParameters;
                    break;
            }

            cameraState = state;
            l_stateTransitionTime = 0;
        }
    }
}