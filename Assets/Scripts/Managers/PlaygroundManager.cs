using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundManager : MonoBehaviour
{
    GameObject[] tracks;
    int currentTrack;

    void Start()
    {
        tracks = new GameObject[transform.childCount];
        for(int i = 0; i< transform.childCount; i++)
        {
            tracks[i] = transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        //CHANGE TRACK
        var n = Input.inputString;
        switch (n)
        {
            case "1":
                tracks[currentTrack].SetActive(false);
                currentTrack = 0;
                break;

            case "2":
                tracks[currentTrack].SetActive(false);
                currentTrack = 1;
                break;

            case "3":
                tracks[currentTrack].SetActive(false);
                currentTrack = 2;
                break;

            case "4":
                tracks[currentTrack].SetActive(false);
                currentTrack = 3;
                break;

            case "5":
                tracks[currentTrack].SetActive(false);
                currentTrack = 4;
                break;

            case "6":
                tracks[currentTrack].SetActive(false);
                currentTrack = 5;
                break;

            case "7":
                tracks[currentTrack].SetActive(false);
                currentTrack = 6;
                break;

            case "8":
                tracks[currentTrack].SetActive(false);
                currentTrack = 7;
                break;

            case "9":
                tracks[currentTrack].SetActive(false);
                currentTrack = 8;
                break;
        }

        tracks[currentTrack].SetActive(true);
    }
}
