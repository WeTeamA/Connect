using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectShader : MonoBehaviour
{
    // Start is called before the first frame update
    public bool Connected;
    public float time = 0;
    public float speed = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Connected)
        {
            time += Time.deltaTime;
            gameObject.GetComponent<MeshRenderer>().materials[0].SetFloat("_ScriptableTime", time*speed);
        }
        else
        {
            time = 0;
        }
    }
}
