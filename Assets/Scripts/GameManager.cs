using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //SINGLETON
    public static GameManager gm;
    private void Awake()
    {
        if (gm != null)
            Destroy(this.gameObject);
        else gm = this;

        DontDestroyOnLoad(this.gameObject);
    }

    //EFFECTS
    [System.Serializable]
    public class Effects
    {
        //LIST OF EFFECTS
        public GameObject explosion;
    }
    public Effects effects;

    //UI ELEMENTS
    [System.Serializable]
    public class UIElements
    {
        //LIST OF UI ELEMENTS
        public Text speedometer;
        public Slider energyBar;
    }
    public UIElements UI;

    //REFERENCES
    PlayerMovement player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        //UPDATE UI
        UI.speedometer.text = Mathf.FloorToInt(player.currentSpeed).ToString();
        UI.energyBar.value = player.l_energy / player.energyParameters.maxEnergy;

        //PROTO DEBUG
        if (Input.GetButton("Pause"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void InstantiateEffect(string effect, Vector3 position, Quaternion rotation)
    {
        switch (effect)
        {
            case "Explosion":
                Instantiate(effects.explosion, position, rotation);
                break;
        }
    }
}
