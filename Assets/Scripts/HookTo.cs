using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HookToObject
{
    public float DistanceToPlayer;
    public GameObject player;
    public float acceleration = 10;
}


public class HookTo : MonoBehaviour
{

    public HookToObject HookToObject;
    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<Rigidbody>()) //Adds ridibody with components
        {
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }
        if (GetComponent<Collider>())
        {
            gameObject.AddComponent<MeshCollider>();
        }

        gameObject.tag = "hookTo";  //Allpy's correct tag, so player yag could find it
        HookToObject.player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {

        HookToObject.DistanceToPlayer = Vector3.Distance(transform.position, HookToObject.player.transform.position);
    }
}
