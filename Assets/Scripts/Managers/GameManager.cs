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
    [HideInInspector] public string previousScene;

    //SINGLETON
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(this.gameObject);
        else Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    //PLAYER SPACESHIP SPECS
    [HideInInspector] public Spaceship playerSpecs;

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

    public void LoadRace(Spaceship specs) //EN EL FUTURO ESTA FUNCIÓN PEDIRÁ CIRCUITO Y DEMÁS COSAS
    {
        playerSpecs = specs;
        SceneManager.LoadScene("RaceTrackLarge(DOUBLE LINES)");
    }

    public void LoadRaceCustomTier() //DEBUG
    {
        playerSpecs = new Spaceship();
        playerSpecs.maxSpeedValue = float.Parse(GameObject.Find("maxSpeedIF").GetComponent<InputField>().text);
        playerSpecs.maxAccelerationvalue = float.Parse(GameObject.Find("accelerationIF").GetComponent<InputField>().text);
        playerSpecs.handlingValue = float.Parse(GameObject.Find("handlingSpeedIF").GetComponent<InputField>().text);
        playerSpecs.cadenceValue = 0.2f; //HARDCODED DE MOMENTO

    }

    public void DebuggingChangeScene(string sceneName)
    {
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);

    }
}
