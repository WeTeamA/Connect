using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public float StandartSpeed = 1;
    /// <summary>
    /// Acceleration when rotates to orbit
    /// </summary>
    public float MoveAccelerateMultiply = 5;
    /// <summary>
    /// Rotation Speed
    /// </summary>
    public float RotationForce = 200;
    public Vector3 AngularVelocity;
    public Vector3 Velocity;
    public float AngleX;
    public float AngleToHookTo;



}

public class Player : MonoBehaviour
{

    List<HookTo> hookTo; //Contains all hook to objects
    public PlayerStats playerStats; //Functional and changable properties
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
        if (!GetComponent<Rigidbody>()) //Adds ridibody with components
        {
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.angularDrag = 49;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        if (!GetComponent<ConstantForce>()) //Adds ConstantForce
        {
            gameObject.AddComponent<ConstantForce>();
        }
        if (!GetComponent<Collider>()) //Adds Collider
        {
            gameObject.AddComponent<SphereCollider>();
        }
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
        if (Hinje)
        {
            HookActionHinje();
        }
        if (Spring)
        {
            HookActionSpring();
        }
        PlayerMovement();
        StatsCollect();


    }


    void StatsCollect() //Calculates all stats for player
    {
        playerStats.AngularVelocity = GetComponent<Rigidbody>().angularVelocity;
        playerStats.Velocity = GetComponent<Rigidbody>().velocity;
        if (lookAtObj)
        {
            lookAtObj.transform.LookAt(closestHookTo.transform, Vector3.forward);
        }
        playerStats.AngleX = (Vector3.SignedAngle(transform.forward, lookAtObj.transform.forward, Vector3.right));
        playerStats.AngleToHookTo = Vector3.Angle(lookAtObj.transform.forward, transform.forward) - 90;
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
                closestHookTo.gameObject.GetComponent<MeshRenderer>().material = mat1; //Disable effects here
            }
        }
        closestHookTo = hookTo[i];
        closestHookTo.gameObject.GetComponent<MeshRenderer>().material = mat2; //Apply effects here

    }


    //for detecting when to connect to hookTo object
    float LastFrameDist;


    void HookActionHinje() //Connects to closest hookTo by hinje joint
    {
        if (Input.GetKeyDown("space"))
        {
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer; //for attach calculations
        }
        if (Input.GetKeyUp("space"))
        {
            GetComponent<ConstantForce>().torque = Vector3.zero; //stops rotating
            if (gameObject.GetComponent<HingeJoint>())
            {
                Destroy(gameObject.GetComponent<HingeJoint>()); //destriys connections
            }
        }

        if (Input.GetKey("space"))
        {            
            if(closestHookTo.HookToObject.DistanceToPlayer > LastFrameDist) //if player moves far away from hookTO
            {
                if (!gameObject.GetComponent<HingeJoint>())
                {
                    if (playerStats.AngleToHookTo < 10 && playerStats.AngleToHookTo > -10)
                    {
                        gameObject.AddComponent<HingeJoint>().anchor = transform.InverseTransformPoint(closestHookTo.transform.position); // when to attach
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
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer; //for attach calculations

            if (gameObject.GetComponent<HingeJoint>())
            {
                GetComponent<ConstantForce>().force *= closestHookTo.HookToObject.acceleration; // Add force when attached
            }
        }
        else
        {
            FindClosestObj();
        }
    }



    void HookActionSpring() //Connects to closest hookTo by spring
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
