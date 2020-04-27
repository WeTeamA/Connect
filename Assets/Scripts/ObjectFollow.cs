using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    Vector3 LastPos;

    // Update is called once per frame
    void Update()
    {
        if (player && LastPos != Vector3.zero)
        {
            LastPos -= player.transform.position;
            transform.position -= LastPos;
        }
        LastPos = player.transform.position;

    }
}
