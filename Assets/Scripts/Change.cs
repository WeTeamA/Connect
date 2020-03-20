using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change : MonoBehaviour
{
    // Start is called before the first frame update
    public float a = 0;
    void Start()
    {
        
    }

    public void Slider_Changed(float x)
    {
        gameObject.GetComponent<MeshRenderer>().materials[0].SetFloat("_Power", x);    
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Moved)
            {
                a += Time.deltaTime;
                gameObject.GetComponent<MeshRenderer>().materials[0].SetFloat("_Power", a);

            }
        }
    }
}
