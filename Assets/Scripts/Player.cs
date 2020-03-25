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
    public float CurrentSpeed;
    public float CurrentAngularSpeed;

    //For calculations
    public float AngleX;
    public float AngleToHookTo;



}

public class Player : MonoBehaviour
{

    List<HookTo> hookTo; //Contains all hook to objects
    [SerializeField]
    bool isAndroidControll;
    public PlayerStats playerStats; //Functional and changable properties
    public HookTo closestHookTo; //Closest hook to at that moment

    [SerializeField] ParticleSystem DeathExplosion;

    [SerializeField] GameObject Connecton; //ConnectionPrefab
    GameObject CurrentConnection; 

    //Testing Grabling Methods
    [SerializeField] bool Hinje = true;

    HookTo LastConnected; //for applyings aditional force when closest hookTo changed

    //Test Only!!!!
    //[SerializeField] Material mat1;
    //[SerializeField] Material mat2;


    //Created lookAt object to simulate normal vector. Have no idea, how to do that without
    GameObject lookAtObj;

    // Start is called before the first frame update
    void Start()
    {

        tag = "Player";

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
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("hookTo"))
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
        else
        {
            HookActionSpring();
        }
        PlayerMovement();
        StatsCollect();
        //ConnectionControll();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "hookTo")
        {

            PlayerDeath();
            gameObject.SetActive(false);

        }
    }

    

    void StatsCollect() //Calculates all stats for player
    {
        playerStats.AngularVelocity = GetComponent<Rigidbody>().angularVelocity;
        playerStats.Velocity = GetComponent<Rigidbody>().velocity;
        playerStats.CurrentSpeed = playerStats.Velocity.magnitude;
        playerStats.CurrentAngularSpeed = playerStats.AngularVelocity.magnitude;
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
            if (hookTo[i].HookToObject.DistanceToPlayer < minDist)
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
                //closestHookTo.gameObject.GetComponent<MeshRenderer>().material = mat1; //Disable effects here
            }
        }
        closestHookTo = hookTo[i];
        //closestHookTo.gameObject.GetComponent<MeshRenderer>().material = mat2; //Apply effects here
    }

    void HookActionHinje() //Connects to closest hookTo by hinje joint
    {

        if (isAndroidControll)
        {
            HookAndroid();
        }
        else
        {
            HookPC();
        }
    }

    void HookPC()
    {
        //For PC
        if (Input.GetKeyDown("space"))
        {
            OnDown();
        }

        if (Input.GetKeyUp("space"))
        {
            OnUp();
        }

        if (Input.GetKey("space"))
        {
            OnPress(true);
        }
        else
        {
            OnPress(false);
        }
    }



    //for detecting when to connect to hookTo object
    float LastFrameDist;

    void HookAndroid()
    {
        //For Android

        foreach (Touch touch in Input.touches)
        {
            if (Input.touches.Length == 1)
            {
                if (touch.phase == TouchPhase.Began) //Started Touch
                {
                    OnDown();
                }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) //Stopped Touch
                {
                    OnUp();
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) //OnTouch
                {
                    OnPress(true);
                }
            }
        }
        if (Input.touches.Length == 0)
        {
            OnPress(false); //Searches when already attached
        }
    }


    void OnDown()
    {
        LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer; //for attach calculations
    }

    void OnUp()
    {
        GetComponent<ConstantForce>().torque = Vector3.zero; //stops rotating
        if (gameObject.GetComponent<HingeJoint>())
        {
            Destroy(gameObject.GetComponent<HingeJoint>()); //destriys connections
        }
        ConnectionControll(false);
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            ConnectionControll(true);
            if (closestHookTo.HookToObject.DistanceToPlayer > LastFrameDist) //if player moves far away from hookTO
            {
                if (!gameObject.GetComponent<HingeJoint>())
                {
                    if (playerStats.AngleToHookTo < 10 && playerStats.AngleToHookTo > -10)
                    {
                        gameObject.AddComponent<HingeJoint>().anchor = transform.InverseTransformPoint(closestHookTo.transform.position); // when to attach
                        gameObject.GetComponent<HingeJoint>().connectedBody = closestHookTo.GetComponent<Rigidbody>();
                        if (LastConnected != closestHookTo)
                        {
                            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * closestHookTo.HookToObject.acceleration); //Adds force when connected
                            LastConnected = closestHookTo;
                        }

                        GetComponent<ConstantForce>().torque = Vector3.zero; //stops rotation force after connection

                        ConnectionPlacmentCalculations();  //Places connection line
                    }
                    else
                    {
                        if (playerStats.AngleX > 0)
                        {
                            GetComponent<ConstantForce>().torque = new Vector3(playerStats.RotationForce, 0, 0); // по часовой
                            GetComponent<ConstantForce>().force += lookAtObj.transform.forward * playerStats.MoveAccelerateMultiply;
                        }
                        else
                        {
                            GetComponent<ConstantForce>().torque = new Vector3(-playerStats.RotationForce, 0, 0); // против часовой
                            GetComponent<ConstantForce>().force += lookAtObj.transform.forward * playerStats.MoveAccelerateMultiply;
                        }
                    }
                }
            }
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer; //for attach calculations

            if (gameObject.GetComponent<HingeJoint>())
            {
                GetComponent<ConstantForce>().force *= closestHookTo.HookToObject.acceleration; // Add force when attached
            }

            if (GetComponent<HingeJoint>())
            {
                FindClosestObj(); //Searches when already attached
            }
        }
        else
        {
            FindClosestObj(); //Searches when already attached
        }

    }

    void ConnectionControll(bool create) //For connection line placment
    {
        if (create) //create to do 2 metods in one
        {
            if (!CurrentConnection) //for not spaming. We use function OnPress to create and OnDown to change position
            {
                CurrentConnection = Instantiate(Connecton, Vector3.Lerp(transform.position, closestHookTo.transform.position, 0.5f), Connecton.transform.rotation, transform);
            }
            if (!gameObject.GetComponent<HingeJoint>()) //We need to change position and rotation only when we on connection process. When we hookedTo, Hierarcy does its job
            {
                ConnectionPlacmentCalculations(); 
            }
            else
            {
                CurrentConnection.transform.position = Vector3.Lerp(transform.position, GetComponent<HingeJoint>().connectedBody.transform.position, 0.5f); //places object in the middle
            }
        }
        else //for destroying
        {
            Destroy(CurrentConnection);
        }
    }

    void ConnectionPlacmentCalculations()
    {
        CurrentConnection.transform.position = Vector3.Lerp(transform.position, closestHookTo.transform.position, 0.5f); //place object in the middle

        //Deals with rotation. Need to remember that method
        Vector3 relativePos = closestHookTo.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        CurrentConnection.transform.rotation = rotation;
        CurrentConnection.transform.up = CurrentConnection.transform.forward;
        CurrentConnection.transform.Rotate(new Vector3(0, 90, 0));

        // Difference in Distance between player/hookTO and both ends on connection object
        float Difference = Vector3.Distance(CurrentConnection.transform.position, closestHookTo.transform.position) /
            Vector3.Distance(CurrentConnection.transform.GetChild(0).transform.position, CurrentConnection.transform.GetChild(1).transform.position);
        Vector3 newScale = CurrentConnection.transform.localScale;
        newScale.y *= Difference * 2;
        CurrentConnection.transform.localScale = newScale;
    }

    void PlayerDeath() //DeathAction
    {
        Instantiate(DeathExplosion, transform.position, transform.rotation).Play();
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
