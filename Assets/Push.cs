using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Push : MonoBehaviour
{
    public GameObject Snar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Instantiate(Snar).GetComponent<Rigidbody>().AddForce(Vector3.forward * 500);
                transform.LookAt(new Vector3(1, 1, 1));
            }
        }
       
      
    }
}
