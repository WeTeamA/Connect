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

    public GameObject PlayerModel;
    [ColorUsage(true, true)]
    public Color color;

    //for test
    public float minVelocity = 100;
    public float maxVelocity = 0;

    //Slow motion
    public float SlowDownFactor;
    public float SlowDownLength;


    //Spring
    public float StartSpring = 10;
    public float TargetSpring = 10;
    public float springModifier;

    public float StartDamper = 0.2f;
    public float TargetDamper = 0.2f;
    public float damperModifier;

    public float NewAngle;






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



    [SerializeField] HookTo LastConnected; //for applyings aditional force when closest hookTo changed

    //Test Only!!!!
    [SerializeField] bool Hinje;



    //Created lookAt object to simulate normal vector. Have no idea, how to do that without
    GameObject lookAtObj;

    GameObject RotatedLookAt;

    // Start is called before the first frame update
    void Start()
    {

        tag = "Player";

        //Adding necessary components
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

        //
        RotatedLookAt = Instantiate(new GameObject(), transform.position, transform.rotation);
        RotatedLookAt.AddComponent<ObjectFollow>();

        //

    }




    float time; //For starting death after time
    private void FixedUpdate()
    {

        PlayerMovement();

        HookAction();
        //PlayerMovement();
        StatsCollect();
        SlowMotionControll(); //For normalasing time after slow motion effect

        //Death
        time += Time.deltaTime;
        if (time > 2)
        {
            if (playerStats.minVelocity > playerStats.CurrentSpeed)
            {
                playerStats.minVelocity = playerStats.CurrentSpeed;
            }
        }
        //
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == "AstroParts")
        {
            DeathControll();
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
        if (RotatedLookAt)
        {
            if (GetComponent<Joint>())
            {
                RotatedLookAt.transform.LookAt(GetComponent<Joint>().connectedBody.transform);
            }
            else
            {
                RotatedLookAt.transform.LookAt(closestHookTo.transform);
            }
            RotatedLookAt.transform.up = RotatedLookAt.transform.forward;
        }

        playerStats.AngleX = (Vector3.SignedAngle(transform.forward, lookAtObj.transform.forward, Vector3.right));
        playerStats.color = playerStats.PlayerModel.GetComponent<ParticleSystemRenderer>().material.GetColor("_EmissionColor");


        Vector3 relativePos = closestHookTo.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        playerStats.AngleToHookTo = Vector3.Angle(RotatedLookAt.transform.forward, transform.forward);
        playerStats.AngleToHookTo = playerStats.AngleToHookTo > 90 ? 180 - playerStats.AngleToHookTo : playerStats.AngleToHookTo;


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
                closestHookTo.gameObject.GetComponent<HookTo>().HookToObject.IsClosest = false; //Disable effects here
            }
        }
        closestHookTo = hookTo[i];
        closestHookTo.gameObject.GetComponent<HookTo>().HookToObject.IsClosest = true; //Apply effects here
    }

    void HookAction() //Connects to closest hookTo by hinje joint
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
            OnDownSpring();

        }

        if (Input.GetKeyUp("space"))
        {
            OnUpSpring();

        }

        if (Input.GetKey("space"))
        {
            OnPressSpring(true);

        }
        else
        {
            OnPressSpring(false);

        }
    }





    void HookAndroid()
    {
        //For Android

        foreach (Touch touch in Input.touches)
        {
            if (Input.touches.Length == 1)
            {
                if (touch.phase == TouchPhase.Began) //Started Touch
                {
                    OnDownSpring();
                }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) //Stopped Touch
                {
                    OnUpSpring();
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) //OnTouch
                {
                    OnPressSpring(true);
                }
            }
        }
        if (Input.touches.Length == 0)
        {
            OnPressSpring(false); //Searches when already attached
        }
    }


 
    //Last and Pre last are for detecting wether we need hinge joint or spring joint to connect
    //for detecting when to connect to hookTo object
    float LastFrameDist;
    //for detecting when to connect to hookTo object with hinje
    float PreLastFrameDist;

    void OnDownSpring()
    {
        LastFrameDist = 0; //for attach calculations
    }

    void OnUpSpring()
    {
        GetComponent<ConstantForce>().torque = Vector3.zero; //stops rotating
        if (gameObject.GetComponent<Joint>())
        {
            Destroy(gameObject.GetComponent<Joint>()); //destriys connections
        }
        ConnectionControllSpring(false);
        if (LastConnected != closestHookTo)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * closestHookTo.HookToObject.OnConnectionForce); //Adds force when connected
            LastConnected = closestHookTo;
        }
    }


    float TimeOnRightPos;
    void OnPressSpring(bool isPressed)
    {
        if (isPressed)
        {
            ConnectionControllSpring(true);
            if (closestHookTo.HookToObject.DistanceToPlayer > LastFrameDist && LastFrameDist != 0) //if player moves far away from hookTO
            {
                if (!gameObject.GetComponent<Joint>())
                {
                    if (playerStats.AngleToHookTo < 11)
                    {

                        if (LastFrameDist < PreLastFrameDist)
                        {
                            gameObject.AddComponent<HingeJoint>().anchor = transform.InverseTransformPoint(closestHookTo.transform.position); // when to attach
                            gameObject.GetComponent<HingeJoint>().connectedBody = closestHookTo.GetComponent<Rigidbody>();
                        }
                        else
                        {
                            gameObject.AddComponent<SpringJoint>().anchor = transform.InverseTransformPoint(closestHookTo.transform.position); // when to attach
                            gameObject.GetComponent<SpringJoint>().spring = playerStats.StartSpring;
                            gameObject.GetComponent<SpringJoint>().damper = playerStats.StartDamper;
                            gameObject.GetComponent<SpringJoint>().connectedBody = closestHookTo.GetComponent<Rigidbody>();
                        }

                        SlowMotionControll(true); // StartSlowMotion

                        GetComponent<ConstantForce>().torque = Vector3.zero; //stops rotation force after connection

                        ConnectionPlacmentCalculations();  //Places connection line
                    }
                    else
                    {
                        if (playerStats.AngleX > 0)
                        {
                            GetComponent<ConstantForce>().torque = new Vector3(playerStats.RotationForce, 0, 0); // по часовой
                        }
                        else
                        {
                            GetComponent<ConstantForce>().torque = new Vector3(-playerStats.RotationForce, 0, 0); // против часовой
                        }
                    }
                }
            }
            PreLastFrameDist = LastFrameDist;
            LastFrameDist = closestHookTo.HookToObject.DistanceToPlayer; //for attach calculations

            if (GetComponent<Joint>())
            {
                GetComponent<ConstantForce>().force *= closestHookTo.HookToObject.acceleration; // Add force when attached
                FindClosestObj(); //Searches when already attached

            }

            if (GetComponent<SpringJoint>())
            {
                //FindClosestObj(); //Searches when already attached
                if(GetComponent<SpringJoint>().spring < playerStats.TargetSpring)
                {
                    GetComponent<SpringJoint>().spring *= playerStats.springModifier;
                }
                if (playerStats.AngleToHookTo < 25)
                {
                    TimeOnRightPos += Time.deltaTime;
                }
                else
                {
                    TimeOnRightPos = 0;
                }
                if(TimeOnRightPos > 1 && playerStats.AngleToHookTo < 11)
                {
                    TimeOnRightPos = 0;
                    gameObject.AddComponent<HingeJoint>().anchor = transform.InverseTransformPoint(GetComponent<SpringJoint>().connectedBody.transform.position); // when to attach
                    gameObject.GetComponent<HingeJoint>().connectedBody = GetComponent<SpringJoint>().connectedBody.GetComponent<Rigidbody>();
                    Destroy(GetComponent<SpringJoint>());
                }
                
            }
        }
        else
        {
            FindClosestObj(); //Searches when not attached
        }

    }



    void ConnectionControllSpring(bool create) //For connection line placment
    {
        if (create) //create to do 2 metods in one
        {
            if (!CurrentConnection) //for not spaming. We use function OnPress to create and OnDown to change position
            {
                CurrentConnection = Instantiate(Connecton, Vector3.Lerp(transform.position, closestHookTo.transform.position, 0.5f), Connecton.transform.rotation);
                CurrentConnection.AddComponent<ObjectFollow>();

                //Setting Color of Connection
                CurrentConnection.GetComponent<MeshRenderer>().material.SetColor("_BallColor", playerStats.color);
                CurrentConnection.GetComponent<MeshRenderer>().material.SetColor("_AsteroidColor", closestHookTo.HookToObject.color);

            }
            /*if (!gameObject.GetComponent<Joint>()) //We need to change position and rotation only when we on connection process. When we hookedTo, Hierarcy does its job
            {
                ConnectionPlacmentCalculations();
            }
            else
            {
                CurrentConnection.transform.position = Vector3.Lerp(transform.position, GetComponent<Joint>().connectedBody.transform.position, 0.5f); //places object in the middle
            }*/
            ConnectionPlacmentCalculations();
        }
        else //for destroying
        {
            Destroy(CurrentConnection);
        }
    }

    void ConnectionPlacmentCalculations()
    {

        Vector3 vector3;
        if (!gameObject.GetComponent<Joint>())
        {
            vector3 = closestHookTo.transform.position;
            //CurrentConnection.transform.position = Vector3.Lerp(transform.position, closestHookTo.transform.position, 0.5f); //place object in the middle
        }
        else

        {
            vector3 = GetComponent<Joint>().connectedBody.transform.position;
            //CurrentConnection.transform.position = Vector3.Lerp(transform.position, GetComponent<Joint>().connectedBody.transform.position, 0.5f);
        }

        CurrentConnection.transform.position = Vector3.Lerp(transform.position, vector3, 0.5f); //place object in the middle

        //Deals with rotation. Need to remember that method

        Vector3 relativePos = vector3 - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        CurrentConnection.transform.rotation = rotation;
        CurrentConnection.transform.up = CurrentConnection.transform.forward;
        CurrentConnection.transform.Rotate(new Vector3(0, 90, 0));

        


        // Difference in Distance between player/hookTO and both ends on connection object
        float Difference = Vector3.Distance(CurrentConnection.transform.position, vector3) /
            Vector3.Distance(CurrentConnection.transform.GetChild(0).transform.position, CurrentConnection.transform.GetChild(1).transform.position);
        Vector3 newScale = CurrentConnection.transform.localScale;
        newScale.y *= Difference * 2;
        CurrentConnection.transform.localScale = newScale;

        if (GetComponent<HingeJoint>() && CurrentConnection.GetComponent<ObjectFollow>())
        {
            CurrentConnection.transform.parent = transform;
            Destroy(CurrentConnection.GetComponent<ObjectFollow>());
        }
    }

    void DeathControll() //DeathAction
    {
        if (playerStats.CurrentSpeed < 0.4f)
        {
            Instantiate(DeathExplosion, transform.position, transform.rotation).Play();
            gameObject.SetActive(false);
        }
    }

    void SlowMotionControll(bool StartSlowMo = false)
    {

        if (StartSlowMo)
        {
            Time.timeScale = playerStats.SlowDownFactor;
        }
        else
        {
            Time.timeScale += (1f / playerStats.SlowDownLength) * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0, 1);
            Time.fixedDeltaTime = 0.01f * Time.timeScale;
        }
        print("scale " +Time.timeScale);
        print("fixed " + Time.fixedDeltaTime);

    }


}
