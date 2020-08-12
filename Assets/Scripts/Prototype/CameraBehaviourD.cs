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
    PlayerMovement player;

    //CORE PARAMETERS
    float fieldOfView;
    float distanceToTarget;

    //------------------------------------ EXTRA SETTINGS
    //PUBLIC ON INSPECTOR
    [System.Serializable]
    public class ExtraSettings
    {
        public bool staticMode = true;
    }
    public ExtraSettings extraSettings;
    //---------------------------------------------------

    //OTHER
    float offsetX, offsetY, offsetZ;

    void Start()
    {
        //REFERENCES
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        //SET OFFSETS
        offsetX = transform.position.x - player.transform.position.x;
        offsetY = transform.position.y - player.transform.position.y;
        offsetZ = transform.position.z - player.transform.position.z;
    }

    void LateUpdate()
    {
        transform.forward = player.forwardDirection;

        //STATE MACHINE
        switch (cameraState)
        {
            case CameraState.idle:
                distanceToTarget = 5;
                fieldOfView = 75;
                break;
        }

        //CAMERA MODE
        switch (cameraMode)
        {
            case CameraMode.staticMode:
                transform.position = new Vector3(player.transform.position.x + offsetX, transform.position.y, transform.position.z);
                break;

            case CameraMode.dynamicMode:
                transform.position = player.transform.position - transform.forward * distanceToTarget;
                transform.LookAt(player.transform.position);
                transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                break;
        }

        ////STATIC MODE?
        //if (extraSettings.staticMode)
        //    transform.position = new Vector3(player.position.x + offsetX, transform.position.y, transform.position.z);

        //else transform.position = new Vector3(player.position.x + offsetX, player.position.y + offsetY, player.position.z + offsetZ);
    }
}
