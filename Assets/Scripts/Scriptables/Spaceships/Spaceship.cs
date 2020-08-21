using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship", menuName = "ScriptableObjects/SpaceshipScriptableObject", order = 1)]
public class Spaceship : ScriptableObject
{
    public Mesh spaceshipMesh;
    public float speed;
    public float acceleration;
}
