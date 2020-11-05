using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewWorkshopManager : MonoBehaviour
{
    public GameObject transitionMaster;
    float transitionTime;

    public void ChangeScene(string sceneToTransition)
    {
        StartCoroutine("SceneTransition", sceneToTransition);
    }

    IEnumerator SceneTransition(string sceneToTransition)
    {
        // SET OUTRO ANIMATION
        transitionMaster.GetComponent<Animation>().clip = transitionMaster.GetComponent<Animation>().GetClip("WorkshopOutroAnimation_Test");
        transitionTime = transitionMaster.GetComponent<Animation>().clip.length;
        transitionMaster.GetComponent<Animation>().Play();

        //WAIT ANIMATION TO END
        yield return new WaitForSeconds(transitionTime);

        //CHANGE SCENE
        SceneManager.LoadScene(sceneToTransition);
    }
}
