using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmBehaviourQTE : MonoBehaviour
{
    //RECT REFERENCES
    public RectTransform checkMark;
    public RectTransform greenZone;
    public RectTransform yellowZone;

    //TECHNICAL VALUES
    public float boostZoneWidth;
    public float checkMarkSpeed;
    float decelerationValue;
    float yellowZoneScaleValue , greenZoneScaleValue;
    
    //SUCCESS ZONE VALUES
    public bool successQTE;
    Vector2 successZone = Vector2.zero;

    void Start()
    {
        //SET YELLOW ZONE SCALE VALUES
        yellowZoneScaleValue = GameManager.Instance.player.jetParameters.boostAcceleration / 60.0f;
        yellowZone.localScale = new Vector2(yellowZoneScaleValue, 1);
        
        //SET GREEN ZONE SCALE VALUES
        greenZoneScaleValue = GameManager.Instance.player.jetParameters.boostAcceleration / 60.0f;
        greenZone.localScale = new Vector2(greenZoneScaleValue, 1);

        //LOCATE ON RANDOM POSITION
        yellowZone.localPosition = new Vector2(50 + Random.Range(0, 47 - yellowZone.rect.width * yellowZone.localScale.x), 0);
        greenZone.localPosition = new Vector2(yellowZone.localPosition.x + (((yellowZone.rect.width * yellowZone.localScale.x)/2)-((greenZone.rect.width * greenZone.localScale.x) / 2)), 0);

        successZone.x = greenZone.localPosition.x;
        successZone.y = greenZone.localPosition.x + greenZone.rect.width;
    }

    void Update()
    {
        if (GameManager.Instance.playerInput.nitroHold && checkMark.localPosition.x < 100)
        {
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
