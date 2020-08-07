using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviourD : MonoBehaviour
{
    public bool staticMode = true;

    Transform player;

    float offsetX, offsetY, offsetZ;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        offsetX = transform.position.x - player.position.x;
        offsetY = transform.position.y - player.position.y;
        offsetZ = transform.position.z - player.position.z;
    }

    void LateUpdate()
    {
        if(staticMode)
            transform.position = new Vector3(player.position.x + offsetX, transform.position.y, transform.position.z);

        else transform.position = new Vector3(player.position.x + offsetX, player.position.y + offsetY, player.position.z + offsetZ);
    }
}
