using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmBehaviourQTE : MonoBehaviour
{
    public RectTransform boostZone;
    public RectTransform checkMark;

    public float boostZoneWidth;
    public float checkMarkSpeed;
    float decelerationValue;
    public bool successQTE;
    Vector2 successZone = Vector2.zero;



    void Start()
    {


        //RANDOM SCALE
        boostZone.localScale = new Vector2(Random.Range(0.4f, 1.4f),1);        
        //LOCATE ON RANDOM POSITION
        boostZone.localPosition = new Vector2(50 + Random.Range(0, 47-boostZone.rect.width),0);
        
        
        successZone.x = boostZone.localPosition.x;
        successZone.y = boostZone.localPosition.x + boostZone.rect.width;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.playerInput.warmUp && checkMark.localPosition.x < 100)
        {
            RaceManager.Instance.countDownReady = true;
            checkMark.localPosition = new Vector2(checkMark.localPosition.x + 0.5f, 0);
            decelerationValue = 0;
        }
        else if (checkMark.localPosition.x > 0)
        {
            decelerationValue += Time.deltaTime;
            checkMark.localPosition = new Vector2(checkMark.localPosition.x - decelerationValue, 0);
        }

        if (checkMark.localPosition.x > successZone.x && checkMark.localPosition.x < successZone.y)
        {
            successQTE = true;
        }
        else successQTE = false;
    }
}
