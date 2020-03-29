using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HookToObject
{
    public float DistanceToPlayer;
    public GameObject player;
    public float acceleration = 10;
    public float OnConnectionForce = 200;
    public float RealibitationForce = 200;
    public List<GameObject> HookToParts;
    public float BreakForce;
    [ColorUsage(true, true)]
    public Color color;

}


public class HookTo : MonoBehaviour
{

    public HookToObject HookToObject;
    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

        }

        gameObject.tag = "hookTo";  //Allpy's correct tag, so player yag could find it
        HookToObject.player = GameObject.FindGameObjectWithTag("Player");
        HookToObject.HookToParts = new List<GameObject>();
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            HookToObject.HookToParts.Add(gameObject.transform.GetChild(i).gameObject); // Adding all parts

            //Setting parts 
            HookToObject.HookToParts[i].GetComponent<FixedJoint>().breakForce = HookToObject.BreakForce;
            HookToObject.HookToParts[i].tag = "AstroParts";


        }
        HookToObject.color = HookToObject.HookToParts[0].GetComponent<MeshRenderer>().material.GetColor("_GlowColor");
        

    }

    private void FixedUpdate()
    {
        HookToObject.DistanceToPlayer = Vector3.Distance(transform.position, HookToObject.player.transform.position);

    }
}
