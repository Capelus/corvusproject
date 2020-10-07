using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBehaviour : MonoBehaviour
{
    GameObject[] portions;

    void Start()
    {
        portions = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            portions[i] = transform.GetChild(i).gameObject;
        }
    }

    public void Destroy()
    {
        GetComponent<BoxCollider>().enabled = false;

        foreach (GameObject p in portions)
        {
            p.transform.parent = null;
            p.GetComponent<Rigidbody>().isKinematic = false;
            p.GetComponent<Rigidbody>().AddExplosionForce(1000, p.transform.position, 5);
        }
    }
 }
