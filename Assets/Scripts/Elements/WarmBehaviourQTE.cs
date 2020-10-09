using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmBehaviourQTE : MonoBehaviour
{

    //RECT REFERENCES
    public RectTransform boostZone;
    public RectTransform checkMark;
    public RectTransform yellowZone;

    //TECHNICAL VALUES
    public float boostZoneWidth;
    public float checkMarkSpeed;
    float decelerationValue;
    float yellowZoneScaleValue , boostZoneScaleValue;
    

    //SUCCESS ZONE VALUES
    public bool successQTE;
    Vector2 successZone = Vector2.zero;



    void Start()
    {
        //SET GREEN & YELLOW ZONE SCALE VALUES
        yellowZoneScaleValue = GameManager.Instance.player.jetParameters.boostAcceleration / 60.0f;
        boostZoneScaleValue = GameManager.Instance.player.jetParameters.boostAcceleration / 60.0f;
        boostZone.localScale = new Vector2(boostZoneScaleValue,1);
        yellowZone.localScale = new Vector2(yellowZoneScaleValue, 1);
 
        //LOCATE ON RANDOM POSITION
        yellowZone.localPosition = new Vector2(50 + Random.Range(0, 47 - yellowZone.rect.width * yellowZone.localScale.x), 0);
        boostZone.localPosition = new Vector2(yellowZone.localPosition.x + (((yellowZone.rect.width*yellowZone.localScale.x)/2)-((boostZone.rect.width*boostZone.localScale.x)/2)),0);

        successZone.x = boostZone.localPosition.x;
        successZone.y = boostZone.localPosition.x + boostZone.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playerInput.nitroHold && checkMark.localPosition.x < 100 && RaceManager.Instance.startSeqEnded)
        {
            RaceManager.Instance.countDownReady = true;
            checkMark.localPosition = new Vector2(checkMark.localPosition.x + 0.5f, 50);
            decelerationValue = 0;
        }
        else if (checkMark.localPosition.x > 0)
        {
            decelerationValue += Time.deltaTime;
            checkMark.localPosition = new Vector2(checkMark.localPosition.x - decelerationValue, 50);
        }

        if (checkMark.localPosition.x > successZone.x && checkMark.localPosition.x < successZone.y)
        {
            successQTE = true;
        }
        else successQTE = false;
    }
}
