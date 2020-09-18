using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarmBehaviourQTE : MonoBehaviour
{
    public RectTransform boostZone;
    public RectTransform checkMark;

    public float boostZoneWidth;
    public float checkMarkSpeed;

    Vector2 succesZone = Vector2.zero;

    void Start()
    {


        //RANDOM SCALE
        boostZone.localScale = new Vector2(Random.Range(0.4f, 1.4f),1);        
        //LOCATE ON RANDOM POSITION
        boostZone.localPosition = new Vector2(50 + Random.Range(0, 47-boostZone.rect.width),0);

    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.Instance.playerInput.accelerate && checkMark.localPosition.x<100)
        {
            checkMark.localPosition = new Vector2(checkMark.localPosition.x + 1 * GameManager.Instance.playerInput.throttle, 0);

        }
        else if (checkMark.localPosition.x > 0)
        {
            checkMark.localPosition = new Vector2(checkMark.localPosition.x -0.25f,0);
        }
        
    }
}
