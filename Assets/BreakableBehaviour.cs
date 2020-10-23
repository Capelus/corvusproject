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
        foreach (GameObject p in portions)
        {
            p.GetComponent<Rigidbody>().isKinematic = false;
            p.GetComponent<Rigidbody>().AddExplosionForce(200, p.transform.position, 10);
            p.GetComponent<Rigidbody>().AddForce(GameManager.Instance.player.transform.forward * 500);
        }
    }
 }
