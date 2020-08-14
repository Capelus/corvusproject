using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : MonoBehaviour
{
    public float speed = 80;
    public float lifeTime = 2;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            Destroy(this.gameObject);
    }
}
