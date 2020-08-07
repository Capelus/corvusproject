using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementC : MonoBehaviour
{

    public float xspeed = 0.8f;
    public float yspeed = 0.1f;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start(){

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float translation = Input.GetAxis("Vertical") * yspeed;
        //Clamper
        if(transform.position.y < 1.5f) transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
        if (transform.position.y > 10) transform.position = new Vector3(transform.position.x, 10.0f, transform.position.z);
        //

        if (transform.position.y >= 1.5f && transform.position.y <= 10.0f){
            transform.Translate(0, Mathf.Clamp(translation,-0.1f,0.1f), 0);
        }


        transform.Translate(0, 0, xspeed);
    }
}
