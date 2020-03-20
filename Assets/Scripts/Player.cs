using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public float StandartSpeed = 1;
    public Vector3 AngularVelocity;
    public Vector3 Velocity;
    public Vector3 ElureAngleToHookTo;
    public float AngleToHookTo;
    public float AngleX;
    public float AngleY;




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


    //Created lookAt object to simulate normal vector. Have no idea, how to do that without
    GameObject lookAtObj;

    // Start is called before the first frame update
    void Start()
    {

        //Finds all hookTo objects by tag
        hookTo = new List<HookTo>();
        foreach(GameObject gameObject in GameObject.FindGameObjectsWithTag("hookTo"))
        {
            hookTo.Add(gameObject.GetComponent<HookTo>());
        }
        //
        lookAtObj = Instantiate(new GameObject(), transform.position, transform.rotation, transform);
        //

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
        //playerStats.AngleToHookTo = GetComponent<Rigidbody>().velocity - transform.forward;

    }




    void PlayerMovement() //Controlls player movement
    {
        GetComponent<ConstantForce>().force = transform.forward * playerStats.StandartSpeed;

        playerStats.ElureAngleToHookTo = lookAtObj.transform.eulerAngles - transform.eulerAngles;
        if (lookAtObj)
        {
            lookAtObj.transform.LookAt(closestHookTo.transform, Vector3.forward);

        }
        playerStats.AngleToHookTo = Vector3.Angle(lookAtObj.transform.forward, transform.forward) - 90;
        playerStats.AngleX = (Vector3.SignedAngle(transform.forward, lookAtObj.transform.forward, Vector3.right));
        playerStats.AngleY = (Vector3.SignedAngle(transform.up, lookAtObj.transform.up, Vector3.right));


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


    void HookActionHinje() //Connects to closest hookTo by hinje joint
    {
        if (Input.GetKeyDown("space"))
        {
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer;
        }
        if (Input.GetKeyUp("space"))
        {
            GetComponent<ConstantForce>().torque = Vector3.zero;
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

                    //lookAtObj.transform.LookAt(closestHookTo.transform);
                    //playerStats.AngleToHookTo = Vector3.Angle(lookAtObj.transform.forward, transform.forward) - 90;
                    //playerStats.ElureAngleToHookTo = lookAtObj.transform.eulerAngles - transform.eulerAngles;

                    if (playerStats.AngleToHookTo < 10 && playerStats.AngleToHookTo > -10)
                    {
                        gameObject.AddComponent<HingeJoint>().anchor = transform.InverseTransformPoint(closestHookTo.transform.position);
                    }
                    else
                    {

                        if (playerStats.AngleX > 0)
                        {
                            GetComponent<ConstantForce>().torque = new Vector3(2000, 0, 0); // по часовой
                            GetComponent<ConstantForce>().force += lookAtObj.transform.forward * 5;
                        }
                        else
                        {
                            GetComponent<ConstantForce>().torque = new Vector3(-2000, 0, 0); // против часовой
                            GetComponent<ConstantForce>().force += lookAtObj.transform.forward * 5;
                        }
                    }
                }
            }
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer;

            if (gameObject.GetComponent<HingeJoint>())
            {
                GetComponent<ConstantForce>().force *= closestHookTo.HookToObject.acceleration;
            }
        }
        else
        {

        }



    }



    void HookActionSpring() //Connects to closest hookTo
    {
        if (Input.GetKeyUp("space"))
        {
            if (gameObject.GetComponent<SpringJoint>())
            {
                Destroy(gameObject.GetComponent<SpringJoint>());
            }
        }
        if (Input.GetKeyDown("space"))
        {
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer;
        }
        if (Input.GetKey("space"))
        {
            if (closestHookTo.HookToObject.DistanceToPlayer > LastFrameDist)
            {
                if (!gameObject.GetComponent<SpringJoint>())
                {
                    SpringJoint springJoint = gameObject.AddComponent<SpringJoint>();
                    springJoint.anchor = transform.InverseTransformPoint(closestHookTo.transform.position);
                    springJoint.maxDistance = 1;
                    springJoint.spring = 100;
                    springJoint.damper = 300;
                }
            }
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer;

            if (gameObject.GetComponent<SpringJoint>())
            {
                GetComponent<ConstantForce>().force *= closestHookTo.HookToObject.acceleration;
            }
        }
    }


}
