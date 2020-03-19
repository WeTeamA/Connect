using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public ParticleSystem Hit;
    //float a;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        Hit.Stop();
        Hit.transform.position = collision.transform.position;
        Hit.transform.rotation = collision.transform.rotation;
       // Hit.transform.Rotate(new Vector3(90, 0, 0));
        Hit.Play();
    }

    // Update is called once per frame
    void Update()
    {

        /*
        a += Time.deltaTime;
        gameObject.transform.Rotate(-a*2f,0,0);
        */
    }
}
