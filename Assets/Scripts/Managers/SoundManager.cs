using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //SINGLETON
    public static SoundManager Instance;

    AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(string name)
    {
        switch (name)
        {
            case "NitroCluster":
                audioSource.PlayOneShot(audioClips[0]);
                break;
        }
    }
}
