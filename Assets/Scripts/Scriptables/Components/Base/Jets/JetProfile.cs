using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jet Profile", menuName = "Components/New Jet Profile", order = 4)]
public class JetProfile : ScriptableObject
{
    //----------------------------------- BOOST
    [Tooltip("The acceleration boost given while performing a boost.")]
    public float boost;

    [Tooltip("The acceleration boost given while performing a superboost.")]
    public float superBoost;

    //---------------------------------- INTAKE
    [Tooltip("The amount of nitro drained when using boosts.")]
    public float boostIntake;

    [Tooltip("The amount of nitro drained when using superboosts.")]
    public float superBoostIntake;
}
