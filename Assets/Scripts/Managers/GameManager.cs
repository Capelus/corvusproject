using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

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

    void Update()
    {
        //PROTO DEBUG
        if (Input.GetButton("Pause"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);        
    }
}
