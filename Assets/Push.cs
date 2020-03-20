using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Push : MonoBehaviour
{
   //public GameObject Snar;
    public ParticleSystem part;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        ParticleSystem p = Instantiate(part);
        p.transform.position = collision.transform.position;
        p.Play();
      //  gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

       
      
    }

    
    
}
