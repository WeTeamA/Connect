﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class HookToObject
{
    [HideInInspector] public float DistanceToPlayer;
    [HideInInspector] public GameObject player;

    [Header("Constant player speed multiplier (when attached). Write")]
    public float acceleration = 10;

    [Header("Force, that we apply after connection ended. Write")]
    public float AfterConnectionForce = 200;
    //public float RealibitationForce = 200;
    [HideInInspector] public List<GameObject> HookToParts;

    [Header("Force, that we need to apply to break asteroid part. Write")]
    public float BreakForce;

    [Header("Asteroid color (for connection). ReadOnly")]
    [ColorUsage(true, true)]
    public Color color;
    [HideInInspector] public bool IsClosest = false; //For understanding, if this asteroid is the closest and it must take selected particle
}


public class HookTo : MonoBehaviour
{

    public HookToObject HookToObject;


    GameObject SelecctedParticle; //shows, that it is the closest

    
    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        SelecctedParticle = GameObject.FindGameObjectWithTag("SelectParticle");

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

    void Desctruction()
    {
        foreach (GameObject part in HookToObject.HookToParts)
        {
            if (!part.GetComponent<Joint>()) //if one of parts breaks
            {
                foreach (GameObject brokenPart in HookToObject.HookToParts)
                {
                    if (brokenPart.GetComponent<Joint>())
                    {

                        Destroy(brokenPart.GetComponent<Joint>());
                    }
                }
                HookToObject.player.GetComponent<Player>().hookTo.Remove(this);
                if (HookToObject.player.GetComponent<Joint>())
                {
                    if (HookToObject.player.GetComponent<Joint>().connectedBody == GetComponent<Rigidbody>())
                    {
                        Destroy(HookToObject.player.GetComponent<Joint>());
                        HookToObject.player.GetComponent<Player>().StopReadingInput = true;
                        HookToObject.player.GetComponent<Player>().ConnectionControllSpring(false);
                    }
                }

            }
        }
    }

    public void TakeParticle()
    {
        if (HookToObject.IsClosest)
        {
            SelecctedParticle.transform.position = Vector3.MoveTowards(SelecctedParticle.transform.position, transform.position, Time.deltaTime * 100);
        }
    }

    private void FixedUpdate()
    {
        HookToObject.DistanceToPlayer = Vector3.Distance(transform.position, HookToObject.player.transform.position);
        TakeParticle();
        Desctruction();
    }
}
