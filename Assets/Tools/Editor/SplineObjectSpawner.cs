using UnityEngine;
using UnityEditor;

public class SplineObjectSpawner : EditorWindow
{
    string objectName = "";
    int objectID = 1;
    float positionOnTrack;
    int quantityToSpawn = 1;
    float distanceBetweenObjects = 5;
    static Vector3 spawnPos;
    GameObject objectToSpawn;

    bool placeMode;
    bool fillMode;

    GameObject spawnMark;
    TrackManager trackManager;

    [MenuItem("Tools/Spline Object Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SplineObjectSpawner));
    }

    private void OnEnable()
    {
        GameObject target = GameObject.Find("Track Manager");
        trackManager = target.gameObject.GetComponent<TrackManager>();

        spawnMark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spawnMark.transform.localScale = new Vector3(20,0.5f,0.5f);
        spawnMark.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Tools/SpawnMark", typeof(Material)) as Material;
    }

    private void OnDestroy()
    {
        DestroyImmediate(spawnMark.gameObject);
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);

        objectToSpawn = EditorGUILayout.ObjectField("Object to spawn", objectToSpawn, typeof(GameObject), false) as GameObject;
        objectName = EditorGUILayout.TextField("Name", objectName);
        objectID = EditorGUILayout.IntField("ID", objectID);

        placeMode = EditorGUILayout.Foldout(placeMode, new GUIContent("Custom placement"), true);
        if (placeMode)
        {
            positionOnTrack = EditorGUILayout.Slider("Position on track", positionOnTrack, 0, trackManager.GetPathLength());
            quantityToSpawn = EditorGUILayout.IntField("Quantity to spawn", quantityToSpawn);
            distanceBetweenObjects = EditorGUILayout.FloatField("Distance between objects", distanceBetweenObjects);

            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            if (GUILayout.Button("Spawn Object"))
            {
                SpawnObjects();
            }
        }

        fillMode = EditorGUILayout.Foldout(fillMode, new GUIContent("Fill track"), true);
        if (fillMode)
        {
            quantityToSpawn = EditorGUILayout.IntField("Quantity to spawn", quantityToSpawn);

            if (GUILayout.Button("Fill Track"))
            {
                FillTrack();
            }
        }
    }

    private void Update()
    {
        if (spawnMark != null)
        {
            spawnMark.transform.position = spawnPos;
            spawnMark.transform.rotation = trackManager.GetRotationAtDistance(positionOnTrack);
        }
    }

    void SpawnObjects()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("Error: Assign an object to be spawned.");
            return;
        }

        if (objectName == null)
        {
            Debug.LogError("Error: Enter a base name for the object.");
            return;
        }


        for (int i = 0; i < quantityToSpawn; i++)
        {

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            newObject.name = objectName + objectID;

            objectID++;
            positionOnTrack += distanceBetweenObjects;
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);  
        }
    }

    void FillTrack()
    {
        float length = trackManager.GetPathLength();

        float spawnDistance = length / quantityToSpawn;

        for (int i = 1; i <= quantityToSpawn; i++)
        {
            spawnPos = trackManager.GetPositionAtDistance(spawnDistance * i);

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(spawnDistance * i));
            newObject.name = objectName + objectID;

            objectID++;
        }
    }
}

