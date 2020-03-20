using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Push : MonoBehaviour
{
   //public GameObject Snar;
    public ParticleSystem Explose;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        ParticleSystem p = Instantiate(Explose);
        p.transform.position = collision.transform.position;
        p.transform.rotation = gameObject.transform.rotation;
        p.transform.Rotate(0, 180, 0);
        p.Play();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

       
      
    }

    
    
}
