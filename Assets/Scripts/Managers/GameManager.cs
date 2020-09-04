using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class GameManager : MonoBehaviour
{
    //PUBLIC INTEREST REFERENCES
    [HideInInspector] public PlayerBehaviour player;
    [HideInInspector] public CameraBehaviour playerCamera;
    [HideInInspector] public PlayerInput playerInput;

    //SINGLETON
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        //PROTO DEBUG
        if (Input.GetButton("Pause"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <param name="factor">The factor you want to slow time by. (eg: 2 -> Time twice as slow)</param>
    public void SlowTime(float factor)
    {
        Time.timeScale *= 1 / factor;
    }

    public void RestoreTime()
    {
        Time.timeScale = 1;
    }
}
