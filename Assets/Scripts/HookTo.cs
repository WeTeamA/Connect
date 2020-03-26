using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HookToObject
{
    public float DistanceToPlayer;
    public GameObject player;
    public float acceleration = 200;
    public List<GameObject> HookToParts;
    [ColorUsage(true, true)]
    public Color color;

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
        HookToObject.HookToParts = new List<GameObject>();
        for (int i = 0; i < gameObject.transform.childCount;  i++)
        {
            HookToObject.HookToParts.Add(gameObject.transform.GetChild(i).gameObject);
        }
        HookToObject.color = HookToObject.HookToParts[0].GetComponent<MeshRenderer>().material.GetColor("_GlowColor");

    }

    // Update is called once per frame
    void Update()
    {

        HookToObject.DistanceToPlayer = Vector3.Distance(transform.position, HookToObject.player.transform.position);
    }
}
