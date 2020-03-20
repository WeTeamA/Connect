using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public Vector3 PreFramePos;
    public Vector3 LastFramePos;
    public Vector3 ResPos;
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        PreFramePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
       
        LastFramePos = transform.position;
        ResPos = LastFramePos - PreFramePos;
        if (ResPos != new Vector3(0,0,0))
        {
            transform.rotation = Quaternion.LookRotation(ResPos);
        }
        PreFramePos = transform.position;
        
        //transform.forward = transform.GetComponent<Rigidbody>().velocity;

    }
}
