using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [Header("Constant player speed (In force). Write")]
    public float StandartSpeed = 1;
    [Header("Force, that rotates player to orbit position, when he fly away. Write")]
    public float RotationForce = 200;
    [HideInInspector] public Vector3 AngularVelocity;
    [HideInInspector] public Vector3 Velocity;
    [Header("Speed of rotation. ReadOnly")]
    public float CurrentSpeed;
    //[Header("Force, that rotates player to orbit position, when he fly away. ReadOnly")]
    public float CurrentAngularSpeed;

    //For calculations
    [HideInInspector] public float AngleX;
    [HideInInspector] public float AngleToHookTo;

    [Header("Player MeshRenderer. Write")]
    public GameObject PlayerModel;

    //[Header("Shows color of player (For connection). ReadOnly")]
    [ColorUsage(true, true)]
    public Color color;

    //for test
    //[Header("Shows minimum velocity in this game. Good for setting death velocity. ReadOnly")]
    //public float minVelocity = 100;

    //Slow motion
    [Header("Strength of slow mo. (x > 1) - faster, (x < 1) - slower. Write")]
    public float SlowDownFactor;
    [Header("How long slowMo will last. Write")]
    public float SlowDownLength;

}
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public float repelForce;
    public float attractForce;
    public float attractDistance;

    Rigidbody rigidbody;
    bool StartSlowMo;

    public static Player player; 

    [HideInInspector] public List<HookTo> hookTo; //Contains all hook to objects
    [Header("True - android input, False - PC input. Write")]
    [SerializeField]
    bool isAndroidControll;
    public PlayerStats playerStats; //Functional and changable properties
    [Header("Show closest asteroid. ReadOnly")]
    public HookTo closestHookTo; //Closest hook to at that moment
    [HideInInspector] public HookTo ConnectedAsteroid;

    [Header("Death particle. Write")]
    [SerializeField] ParticleSystem DeathExplosion;

    [Header("Connection Object. Write")]
    [SerializeField] GameObject Connecton; //ConnectionPrefab
    GameObject CurrentConnection; 

    HookTo LastConnected; //for applyings aditional force when closest hookTo changed


    //Created lookAt object to simulate normal vector. Have no idea, how to do that without
    GameObject lookAtObj;

    GameObject RotatedLookAt;

    [HideInInspector] public bool StopReadingInput; //For situation, when we destroyed asteroid, and need to destroy connections and tricing for a moment. Sets true by destroued HootTo

    bool Connecting;

    private void Awake()
    {
        player = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        Connecting = false;
        tag = "Player";

        rigidbody = GetComponent<Rigidbody>();
        //Setting necessary components
        rigidbody.useGravity = false;
        rigidbody.angularDrag = 49;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

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
        StopReadingInput = false;


        //Add Force At start
        rigidbody.AddForce(transform.forward * 1, ForceMode.Impulse);
    }




    float time; //For starting death after time
    private void FixedUpdate()
    {

        PlayerMovement();
        
        HookAction();
        StatsCollect();
        SlowMotionControll(); //For normalizing time after slow motion effect

        //Death
        time += Time.deltaTime;
        if (time > 2)
        {
            DeathControll();
            
        }
        //
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.transform.tag == "AstroParts")
        {
            //DeathControll();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BrokenPart")
        {
            Destroy(other.gameObject);

            ChangePlayer(0.1f,0.25f,0.1f);
            GameController.manager.IncreaseScore(10);
        }
    }

    void ChangePlayer(float mass, float speed, float size)
    {
        rigidbody.mass += mass;
        playerStats.StandartSpeed += speed;
        Vector3 sizeV = new Vector3(size, size, size);
        transform.localScale += sizeV / 10;
        playerStats.PlayerModel.transform.localScale += sizeV;
    }

    void StatsCollect() //Calculates all stats for player
    {
        playerStats.AngularVelocity = rigidbody.angularVelocity;
        playerStats.Velocity = rigidbody.velocity;
        playerStats.CurrentSpeed = playerStats.Velocity.magnitude;
        playerStats.CurrentAngularSpeed = playerStats.AngularVelocity.magnitude;
        if (lookAtObj && ConnectedAsteroid)
        {
            lookAtObj.transform.LookAt(ConnectedAsteroid.transform, Vector3.forward);
        }
        if (RotatedLookAt)
        {
            if (Connecting)
            {
                RotatedLookAt.transform.LookAt(ConnectedAsteroid.transform);
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
        float minDist = 10000000;

        //if (closestHookTo)
        //    minDist = closestHookTo.HookToObject.DistanceToPlayer;

        int index = 0;

        for (int i = 0; i < hookTo.Count; i++)
        {
            if (hookTo[i].HookToObject.DistanceToPlayer < minDist)
            {
                minDist = hookTo[i].HookToObject.DistanceToPlayer;
                index = i;
            }
        }
        CheckAndChangeClosestObj(index);
    }

    void CheckAndChangeClosestObj(int i)// Sets Closest and allows to apply effects on it
    {
        if (closestHookTo)
        {
            if (closestHookTo != hookTo[i])
            {
                closestHookTo.HookToObject.IsClosest = false; //Disable effects here
            }
        }
        closestHookTo = hookTo[i];
        closestHookTo.HookToObject.IsClosest = true; //Apply effects here
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
            StartConnection();
            print("Down");
        }

        if (Input.GetKeyUp("space"))
        {
            EndConnection();
            print("Up");

        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartSlowMo = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StartSlowMo = false;

        }
        ConnectionCalc();
    }

    void HookAndroid()
    {
        //For Android


        foreach (Touch touch in Input.touches)
        {
            if (Input.touches.Length >= 1)
            {
                if (touch.phase == TouchPhase.Began) //Started Touch
                {
                    StartConnection();
                }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) //Stopped Touch
                {
                    EndConnection();
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) //OnTouch
                {
                    //SpringCalc(true);
                }
            }
            if (Input.touches.Length >= 2)
            {
                if (touch.phase == TouchPhase.Began) //Started Touch
                {
                }
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) //Stopped Touch
                {
                    EndConnection();
                }
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) //OnTouch
                {
                    //SpringCalc(true);
                }
            }
        }
        if (Input.touches.Length == 0)
        {
            //SpringCalc(false); //Searches when already attached
        }
    }


 
    //Last and Pre last are for detecting wether we need hinge joint or spring joint to connect
    //for detecting when to connect to hookTo object
    float LastFrameDist;
    //for detecting when to connect to hookTo object with hinje

    void StartConnection()
    {
        LastFrameDist = 0; //for attach calculations
        Connecting = true;
        ConnectedAsteroid = closestHookTo;
    }

    public void EndConnection()
    {
        GetComponent<ConstantForce>().torque = Vector3.zero; //stops rotating

        ConnectionControll(false);

        if (LastConnected != ConnectedAsteroid && !StopReadingInput) //Add force, if we didnt attached to that asteroid before
        {
            rigidbody.AddForce(transform.forward * ConnectedAsteroid.HookToObject.AfterConnectionForce); //Adds force when connected
            LastConnected = ConnectedAsteroid;
        }

        StopReadingInput = false; //After releasing button or touch we can continue tracking
        Connecting = false;
    }


    void ConnectionCalc()
    {
        if (Connecting)
        {
            ConnectionControll(true);
            if (ConnectedAsteroid.HookToObject.DistanceToPlayer > LastFrameDist && LastFrameDist != 0) //if player moves far away from hookTO
            {

                CalcForce();

                if (playerStats.AngleX > 0)
                {
                    transform.Rotate(new Vector3(playerStats.RotationForce * playerStats.AngleToHookTo, 0, 0)); // по часовой
                }
                else
                {
                    transform.Rotate(new Vector3(-playerStats.RotationForce * playerStats.AngleToHookTo, 0, 0)); // против часовой
                }
            }
            else
                attractDistance = ConnectedAsteroid.HookToObject.DistanceToPlayer / 2;

            LastFrameDist = ConnectedAsteroid.HookToObject.DistanceToPlayer; //for attach calculations

            GetComponent<ConstantForce>().force *= ConnectedAsteroid.HookToObject.acceleration; // Add force when attached

            ConnectionPlacmentCalculations();  

        }        
        FindClosestObj(); //Searches when not attached

    }

    void CalcForce()
    {
        Transform point1 = transform;
        Transform point2 = ConnectedAsteroid.transform;
        Vector3 d = point1.position - point2.position;
        Vector3 direction = d.normalized;

        //Spring3D spring = GetSpring(graph.edges[i]);
        float displacement = attractDistance - d.magnitude;

        point1.GetComponent<Rigidbody>().AddForce(direction * (attractForce * displacement * 0.5f), ForceMode.Acceleration);
    }

    public void ConnectionControll(bool create) //For connection line placment
    {
        if (create) //create to do 2 metods in one (creates and keeps connection)
        {
            if (!CurrentConnection) //for not spaming. We use function OnPress to create and OnDown to change position
            {
                CurrentConnection = Instantiate(Connecton, Vector3.Lerp(transform.position, ConnectedAsteroid.transform.position, 0.5f), Connecton.transform.rotation);
                CurrentConnection.AddComponent<ObjectFollow>();

                //Setting Color of Connection
                CurrentConnection.GetComponent<MeshRenderer>().material.SetColor("_BallColor", playerStats.color);
                CurrentConnection.GetComponent<MeshRenderer>().material.SetColor("_AsteroidColor", ConnectedAsteroid.HookToObject.color);

            }

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
            vector3 = ConnectedAsteroid.transform.position;
        }
        else
        {
            vector3 = GetComponent<Joint>().connectedBody.transform.position;
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

        ChangePlayer(-0.01f * Time.deltaTime,- 0.025f * Time.deltaTime, -0.01f * Time.deltaTime);

        if (playerStats.CurrentSpeed < 0.4f)
        {
            Instantiate(DeathExplosion, transform.position, transform.rotation).Play();
            GameController.manager.EndLevel(true);
            gameObject.SetActive(false);
        }
    }


    void SlowMotionControll()
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


    }


}
