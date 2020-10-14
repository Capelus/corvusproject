using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EffectsManager : MonoBehaviour
{
    //SINGLETON
    public static EffectsManager Instance;

    //EFFECTS
    [System.Serializable]
    public class Effects
    {
        //LIST OF EFFECTS

        //WARP
        public VisualEffect warp;

        //NEBULA
        public MeshRenderer nebula;

        //VARIOUS
        public GameObject explosion;
    }
    public Effects effects;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //UPDATE WARP EFFECT
        WarpEffect();

        //UPDATE NEBULA EFFECT
        NebulaEffect();
    }

    public void WarpEffect()
    {      
        effects.warp.SetFloat("WarpAmount", (GameManager.Instance.player.currentSpeed / GameManager.Instance.player.l_maxSpeed));
    }

    public void NebulaEffect()
    {
        if(GameManager.Instance.player.currentSpeed > GameManager.Instance.player.engineParameters.maxSpeed + 20)
        {
            effects.nebula.enabled = true;
            effects.nebula.material.SetFloat("Dissolve_", 1 / (GameManager.Instance.player.currentSpeed / GameManager.Instance.player.l_maxSpeed));
        }

       else effects.nebula.enabled = false;
    }

    public void InstantiateEffect(string name, Vector3 position, Quaternion rotation)
    {
        switch (name)
        {
            case "Explosion":
                Instantiate(effects.explosion, position, rotation);
                break;
        }
    }
}
