using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distruction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<FixedJoint>() == null)
        {
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_GlowColor", Color.white); 
        }
    }
}
