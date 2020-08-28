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
        [Range(0, 1)]
        public float warpSpeed = 0;

        //NEBULA
        public MeshRenderer nebula;
        public float nebulaSpeed = 0.3f;
        public float nebulaDissolve = 1;
        public bool nebulaActive;

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
        //WARP EFFECT
        effects.warp.SetFloat("WarpAmount", effects.warpSpeed);

        if (effects.nebulaActive)
        {
            effects.nebula.enabled = true;
            effects.nebula.material.SetFloat("Dissolve_", effects.nebulaDissolve);
            effects.nebula.material.SetVector("NebulaSpeed_", new Vector4(0, effects.nebulaSpeed, 0, 0));
        }

        else effects.nebula.enabled = false;
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
