using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SystemMapManager : MonoBehaviour
{
    // PUBLIC REFERENCES
    public GameObject sun;
    public Text sectionName;
    public Text sectionDescription;

    // INPUT PARAMETERS
    public float inputCadence = 0.5f;
    float l_inputCadence;

    // TRANSITION PARAMETERS
    public GameObject transitionMaster;
    float transitionTime;

    // LOCATIONS
    [System.Serializable]
    public class Location
    {
        public GameObject location;
        public string sectionName;
        public string sceneName;
        [TextArea(10, 10)] public string sectionDescription;
    }
    public Location[] locations;
    int currentSelection = 0;

    void Update()
    {
        // INPUT MANAGEMENT
        GetInput();

        // UPDATE SELECTION ARROW
        for (int i = 0; i < locations.Length; i++)
        {
            if (i == currentSelection)
                locations[i].location.transform.GetChild(0).gameObject.SetActive(true);

            else locations[i].location.transform.GetChild(0).gameObject.SetActive(false);
        }

        // UPDATE UI TEXT
        sectionName.text = locations[currentSelection].sectionName;
        sectionDescription.text = locations[currentSelection].sectionDescription;
    }

    void GetInput()
    {
        l_inputCadence -= Time.deltaTime;

        if (Input.GetAxis("Horizontal") == 0)
            l_inputCadence = 0;

        if (Input.GetAxis("Horizontal") > 0.1f && l_inputCadence < 0) // RIGHT
        {
            currentSelection++;
            if (currentSelection > locations.Length - 1)
                currentSelection = 0;

            l_inputCadence = inputCadence;
        }

        else if (Input.GetAxis("Horizontal") < -0.1f && l_inputCadence < 0) // LEFT
        {
            currentSelection--;
            if (currentSelection < 0)
                currentSelection = locations.Length - 1;

            l_inputCadence = inputCadence;
        }

        if(Input.GetButton("Nitro") || Input.GetKeyDown(KeyCode.Return)) // ENTER
        {
            ChangeScene(locations[currentSelection].sceneName);
        }
    }

    public void ChangeScene(string sceneToTransition)
    {
        StartCoroutine("SceneTransition", sceneToTransition);
    }

    IEnumerator SceneTransition(string sceneToTransition)
    {
        // SET OUTRO ANIMATION
        transitionMaster.GetComponent<Animation>().clip = transitionMaster.GetComponent<Animation>().GetClip("SystemMapOutroAnimation_Test");
        transitionTime = transitionMaster.GetComponent<Animation>().clip.length;
        transitionMaster.GetComponent<Animation>().Play();

        //WAIT ANIMATION TO END
        yield return new WaitForSeconds(transitionTime);

        //CHANGE SCENE
        SceneManager.LoadScene(sceneToTransition);
    }
}
