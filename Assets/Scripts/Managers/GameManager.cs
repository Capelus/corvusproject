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

    void Update()
    {
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
