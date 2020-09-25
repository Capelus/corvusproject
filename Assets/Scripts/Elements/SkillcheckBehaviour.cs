using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillcheckBehaviour : MonoBehaviour
{
    public int skillcheckState = 0;

    public void SetSkillcheckState(int state) => skillcheckState = state;
}
