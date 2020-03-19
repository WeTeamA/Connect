using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public ParticleSystem Hit;
    public float a = 250;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        /*
        Hit.Stop();
        Hit.transform.position = collision.transform.position;
        Hit.transform.rotation = collision.transform.rotation;
       // Hit.transform.Rotate(new Vector3(90, 0, 0));
        Hit.Play();
        */
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0, Time.deltaTime*a, 0);
        /*
        a += Time.deltaTime;
        gameObject.transform.Rotate(-a*2f,0,0);
        */
    }
}
