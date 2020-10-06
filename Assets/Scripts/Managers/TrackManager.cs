using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

[ExecuteInEditMode]
public class TrackManager : MonoBehaviour
{
    //SINGLETON
    public static TrackManager Instance;

    //PATH REFERENCES
    PathCreator pathCreator;
    public EndOfPathInstruction endOfPathInstruction;

    //PUBLIC INTEREST REFERENCES
    [Header("Movement Limits")]
    public Vector2 movementLimits = new Vector2(8,4);

    [HideInInspector] public Vector3 objectPoolPosition;
    [HideInInspector] public GameObject [] enemies;
    [HideInInspector] public GameObject [] shots;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        //SET REFERENCES
        pathCreator = GetComponent<PathCreator>();
        objectPoolPosition = GameObject.Find("Object Pool").transform.position;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void Update()
    {
        if(pathCreator == null)
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

    public Vector3 GetEulerRotationAtDistance(float distance)
    {
        Vector3 rotation;
        rotation = pathCreator.path.GetRotationAtDistance(distance, endOfPathInstruction).eulerAngles;

        return rotation;
    }

    public Vector3 GetDirectionAtDistance(float distance)
    {
        Vector3 direction;
        direction = pathCreator.path.GetPointAtDistance(distance + 1, endOfPathInstruction) - pathCreator.path.GetPointAtDistance(distance, endOfPathInstruction);

        return direction;
    }

    public float GetClosestDistanceOnPath(Vector3 location)
    {
        return pathCreator.path.GetClosestDistanceAlongPath(location);
    }

    public float GetPathLength()
    {
        return pathCreator.path.length;
    }
}
