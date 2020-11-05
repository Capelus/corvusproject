using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpacedomeManager : MonoBehaviour
{
    string selectedTier;
    int raceNumber;

    // TRANSITION PARAMETERS
    public GameObject transitionMaster;

    public void RaceSelection(string selectedTier)
    {
        this.selectedTier = selectedTier;
        transitionMaster.GetComponent<Animator>().Play("SpacedomeRaceSelectionAnimation_Test");
    }

    public void LeagueSelection()
    {        
        transitionMaster.GetComponent<Animator>().Play("SpacedomeLeagueSelectionAnimation_Test");
    }

    public void ChangeScene(string sceneToTransition)
    {
        StartCoroutine("SceneTransition", sceneToTransition);
    }

    public void LoadRace(int raceNumber)
    {
        this.raceNumber = raceNumber;
        ChangeScene("Race");
    }

    IEnumerator SceneTransition(string sceneToTransition)
    {
        //CHANGE SCENE
        string raceToLoad = "";
        if (sceneToTransition == "Race")
        {
            switch (selectedTier)
            {
                case "C":
                    switch (raceNumber)
                    {
                        case 1:
                            raceToLoad = "Race C-1";
                            break;

                        case 2:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 3:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 4:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 5:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;
                    }
                    break;

                case "B":
                    switch (raceNumber)
                    {
                        case 1:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 2:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 3:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 4:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 5:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;
                    }
                    break;

                case "A":
                    switch (raceNumber)
                    {
                        case 1:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 2:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 3:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 4:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 5:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;
                    }
                    break;

                case "S":
                    switch (raceNumber)
                    {
                        case 1:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 2:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 3:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 4:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;

                        case 5:
                            Debug.Log("THIS RACE IS NOT YET IMPLEMENTED.");
                            break;
                    }
                    break;
            }

            if (raceToLoad != "")
            {
                transitionMaster.GetComponent<Animator>().Play("SpacedomeLoadRaceAnimation_Test"); // SET OUTRO ANIMATION                                                                                          
                yield return new WaitForSeconds(1); // WAIT ANIMATION TO END
                SceneManager.LoadScene(raceToLoad);
            }

        }

        else
        {
            transitionMaster.GetComponent<Animator>().Play("SpacedomeOutroAnimation_Test"); // SET OUTRO ANIMATION                                                                                          
            yield return new WaitForSeconds(0.5f); // WAIT ANIMATION TO END
            SceneManager.LoadScene(sceneToTransition);
        }
    }
}
