﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TrackManager : MonoBehaviour
{
    //SINGLETON
    public static TrackManager Instance;

    //PATH REFERENCES
     PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pathCreator = GetComponent<PathCreator>();
    }

    public Vector3 GetPositionAtDistance(float distance)
    {
        Vector3 position;
        position = pathCreator.path.GetPointAtDistance(distance, endOfPathInstruction);

        return position;
    }

    public Quaternion GetRotationAtDistance(float distance)
    {
        Quaternion rotation;
        rotation = pathCreator.path.GetRotationAtDistance(distance, endOfPathInstruction);

        return rotation;
    }

    public Vector3 GetDirectionAtDistance(float distance)
    {
        Vector3 direction;
        direction = pathCreator.path.GetPointAtDistance(distance + 1, endOfPathInstruction) - pathCreator.path.GetPointAtDistance(distance, endOfPathInstruction);

        return direction;
    }
}