using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public GameObject pressButtonText;
    public GameObject logo;
    public TrailRenderer[] trails;
    public GameObject[] AIRacers;

    bool transitioning;
    float transitionTime;
    float t;

    void Update()
    {
        if(Input.GetButtonDown("Nitro") || Input.GetKeyDown(KeyCode.Return))
        {
            pressButtonText.SetActive(false);
            logo.GetComponent<Animation>().Play();
            
            transitioning = true;
            transitionTime = logo.GetComponent<Animation>().clip.length;
            t = transitionTime;

            foreach (GameObject racer in AIRacers)
                racer.GetComponentInChildren<MeshRenderer>().enabled = false;      
        }

        if (transitioning)
        {
            t -= Time.deltaTime;
            foreach (TrailRenderer trail in trails)
            {
                trail.widthMultiplier = (t / transitionTime);
            }
        }

        if (t < -1)
            SceneManager.LoadScene("Workshop");
    }
}
