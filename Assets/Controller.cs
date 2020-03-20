using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public GameObject Snar;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Instantiate(Snar).GetComponent<Rigidbody>().AddForce(Vector3.forward * 500000);
                //transform.LookAt(new Vector3(1, 1, 1));
            }
        }
    }
}
