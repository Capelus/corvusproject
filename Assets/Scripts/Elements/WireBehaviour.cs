using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireBehaviour : MonoBehaviour
{
    PlayerBehaviour player;
    public float distanceBeyond = 100;

    private void Start()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        //CALCULATE POSITION
        transform.position = TrackManager.Instance.GetPositionAtDistance(player.distanceTravelled + distanceBeyond);

        //CALCULATE ROTATION
        transform.rotation = TrackManager.Instance.GetRotationAtDistance(player.distanceTravelled + distanceBeyond);
    }
}
