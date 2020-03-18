using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeConnect : MonoBehaviour
{
    float a = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

                a -= Time.deltaTime;
                gameObject.GetComponent<MeshRenderer>().materials[0].SetFloat("_Power", a);


    }
}
