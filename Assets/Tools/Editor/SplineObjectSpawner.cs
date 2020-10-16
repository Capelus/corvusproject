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
    bool nineObjectsMode;

    //NINE POSITION TOGGLE BOOLS
   
    bool upLeft, upMid, upRight;
    bool midLeft, midMid, midRight;
    bool lowLeft, lowMid, lowRight;

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

        //FILL TRACK MODE
        fillMode = EditorGUILayout.Foldout(fillMode, new GUIContent("Fill track"), true);
        if (fillMode)
        {
            quantityToSpawn = EditorGUILayout.IntField("Quantity to spawn", quantityToSpawn);

            if (GUILayout.Button("Fill Track"))
            {
                FillTrack();
            }
        }

        //NINE POSITION GRID MODE
        nineObjectsMode = EditorGUILayout.Foldout(nineObjectsMode, new GUIContent("Nine Position Grid"), true);
        if (nineObjectsMode)
        {
            positionOnTrack = EditorGUILayout.Slider("Position on track", positionOnTrack, 0, trackManager.GetPathLength());
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            upLeft = EditorGUILayout.Toggle("Upper Left Corner",upLeft);
            upMid = EditorGUILayout.Toggle("Upper Mid Corner", upMid);
            upRight = EditorGUILayout.Toggle("Upper Right Corner", upRight);

            midLeft = EditorGUILayout.Toggle("Mid Left Corner", midLeft);
            midMid = EditorGUILayout.Toggle("Mid Mid Corner", midMid);
            midRight = EditorGUILayout.Toggle("Mid Right Corner", midRight);

            lowLeft = EditorGUILayout.Toggle("Low Left Corner", lowLeft);
            lowMid = EditorGUILayout.Toggle("Low Mid Corner", lowMid);
            lowRight = EditorGUILayout.Toggle("Low Right Corner", lowRight);

            if (GUILayout.Button("Spawn"))
            {
                FillGrid();
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

    void FillGrid()
    {
        if (upLeft)
        {
            spawnPos.y += 8;
            spawnPos.z += 8;
            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (upMid)
        {
            spawnPos.y += 8;
            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (upRight)
        {
            spawnPos.y += 8;
            spawnPos.z -= 8;

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (midLeft)
        {
            spawnPos.z += 8;

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (midMid)
        {
            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (midRight)
        {
            spawnPos.z -= 8;

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (lowLeft)
        {
            spawnPos.y -= 8;
            spawnPos.z += 8;

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (lowMid)
        {
            spawnPos.y -= 8;

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }
        if (lowRight)
        {
            spawnPos.y -= 8;
            spawnPos.z -= 8;

            GameObject newObject = Instantiate(objectToSpawn, spawnPos, trackManager.GetRotationAtDistance(positionOnTrack));
            spawnPos = trackManager.GetPositionAtDistance(positionOnTrack);

            newObject.name = objectName + objectID;
            objectID++;
        }



    }
}

