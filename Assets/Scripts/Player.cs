using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public float StandartSpeed = 1;
    public Vector3 AngularVelocity;
    public Vector3 Velocity;
    public Vector3 AngleToHookTo;

}

public class Player : MonoBehaviour
{

    List<HookTo> hookTo; //Contains all hook to objects
    public PlayerStats playerStats;
    public HookTo closestHookTo; //Closest hook to at that moment



    //Testing Grabling Methods
    [SerializeField] bool Hinje;
    [SerializeField] bool Spring;

    //Test Only!!!!
    [SerializeField] Material mat1;
    [SerializeField] Material mat2;

    // Start is called before the first frame update
    void Start()
    {

        //Finds all hookTo objects by tag
        hookTo = new List<HookTo>();
        foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("hookTo"))
        {
            hookTo.Add(gameObject.GetComponent<HookTo>());
        }
        hookTo[2].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;


    }

    // Update is called once per frame
    void Update()
    {
        FindClosestObj();
        PlayerMovement();
        if (Hinje)
        {
            HookActionHinje();
        }
        if (Spring)
        {
            HookActionSpring();
        }
        StatsCollect();


    }


    void StatsCollect()
    {
        playerStats.AngularVelocity = GetComponent<Rigidbody>().angularVelocity;
        playerStats.Velocity = GetComponent<Rigidbody>().velocity;
        playerStats.AngleToHookTo = closestHookTo.transform.forward - transform.forward;

    }

    void PlayerMovement() //Controlls player movement
    {
        GetComponent<ConstantForce>().force = transform.forward * playerStats.StandartSpeed;
    }



    void FindClosestObj() //Finds id of closest hookTo
    {
        float minDist = hookTo[0].HookToObject.DistanceToPlayer;
        ChangeClosestObj(0);
        for (int i = 0; i < hookTo.Count; i++)
        {
            if(hookTo[i].HookToObject.DistanceToPlayer < minDist)
            {
                minDist = hookTo[i].HookToObject.DistanceToPlayer;
                ChangeClosestObj(i);
            }
        }
    }

    void ChangeClosestObj(int i)// Sets Closest and allows to apply effects on it
    {
        if (closestHookTo)
        {
            if (closestHookTo != hookTo[i])
            {
                closestHookTo.gameObject.GetComponent<MeshRenderer>().material = mat1;
            }
        }
        closestHookTo = hookTo[i];
        closestHookTo.gameObject.GetComponent<MeshRenderer>().material = mat2;

    }


    //for detecting when to connect to hookTo object
    float LastFrameDist;

    void HookActionHinje() //Connects to closest hookTo
    {
        if (Input.GetKeyDown("space"))
        {
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer;
        }
        if (Input.GetKeyUp("space"))
        {
            if (gameObject.GetComponent<HingeJoint>())
            {
                Destroy(gameObject.GetComponent<HingeJoint>());
            }
        }
        if (Input.GetKey("space"))
        {            
            if(closestHookTo.HookToObject.DistanceToPlayer > LastFrameDist)
            {
                if (!gameObject.GetComponent<HingeJoint>())
                {
                    gameObject.AddComponent<HingeJoint>().anchor = transform.InverseTransformPoint(closestHookTo.transform.position);
                }
            }
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer;

            if (gameObject.GetComponent<HingeJoint>())
            {
                GetComponent<ConstantForce>().force *= closestHookTo.HookToObject.acceleration;
            }
        }
    }

    void HookActionSpring() //Connects to closest hookTo
    {
        if (Input.GetKeyDown("space"))
        {
            SpringJoint springJoint = gameObject.AddComponent<SpringJoint>();
            springJoint.anchor = transform.InverseTransformPoint(closestHookTo.transform.position);
            springJoint.maxDistance = 1;
            springJoint.spring = 100;
            springJoint.damper = 10;
        }
        if (Input.GetKeyUp("space"))
        {
            if (gameObject.GetComponent<SpringJoint>())
            {
                Destroy(gameObject.GetComponent<SpringJoint>());
            }
        }
        if (Input.GetKey("space"))
        {
            GetComponent<ConstantForce>().force *= closestHookTo.HookToObject.acceleration;
        }
    }


}
