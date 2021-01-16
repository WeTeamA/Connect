using System.Collections;
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

[System.Serializable]
public class Explosion
{
    public float power;
    public float radius;
    public float Count;
}

[RequireComponent(typeof(Rigidbody))]
public class HookTo : MonoBehaviour
{

    public HookToObject HookToObject;

    bool GoToPlayer;
    bool Broken;

    GameObject SelectedParticle; //shows, that it is the closest

    [SerializeField] Explosion explosion;
    
    // Start is called before the first frame update
    void Start()
    {
        

        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        SelectedParticle = GameObject.FindGameObjectWithTag("SelectParticle");

        gameObject.tag = "hookTo";  //Apply's correct tag, so player tag could find it

        HookToObject.player = GameObject.FindGameObjectWithTag("Player");
        HookToObject.HookToParts = new List<GameObject>();



        foreach (Transform part in transform)
        {
            //Setting parts 
            part.GetComponent<FixedJoint>().breakForce = HookToObject.BreakForce;
            part.tag = "AstroParts";

            HookToObject.HookToParts.Add(part.gameObject); // Adding all parts            
        }

        HookToObject.color = HookToObject.HookToParts[0].GetComponent<MeshRenderer>().material.GetColor("_GlowColor");        
    }

    void CheckingStability()
    {
        if(!Broken)
            foreach (GameObject part in HookToObject.HookToParts)
            {

                if (!part.GetComponent<Joint>()) //if one of parts breaks
                {
                    StartCoroutine(DestroyAsteroid());
                    break;
                }
            }

    }

    IEnumerator DestroyAsteroid()
    {
        Broken = true;
        foreach (GameObject brokenPart in HookToObject.HookToParts)
        {
            if (brokenPart.GetComponent<Joint>())
            {
                Destroy(brokenPart.GetComponent<Joint>());
            }
        }

        Player player = HookToObject.player.GetComponent<Player>();

        player.hookTo.Remove(this);

        if(player.ConnectedAsteroid == this)
            player.EndConnection();

        for (int i = 0; i < explosion.Count; i++)
        {
            Explosion();
        }

        yield return new WaitForSeconds(2);

        foreach (GameObject part in HookToObject.HookToParts)
        {
            part.GetComponent<Rigidbody>().isKinematic = true;
            part.tag = "BrokenPart";
            part.layer = 11;
        }
        GoToPlayer = true;
    }

    void Explosion()
    {
        Vector3 explosionPos = transform.position;

        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosion.radius);

        foreach (Collider hit in colliders)
        {
            if(hit.tag == "AstroParts")
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();

                if (rb != null)
                    rb.AddExplosionForce(explosion.power, explosionPos, explosion.radius);
            }
        }
        StopCoroutine(DestroyAsteroid());
    }

    public void TakeParticle()
    {
        if (HookToObject.IsClosest)
        {
            SelectedParticle.transform.position = Vector3.MoveTowards(SelectedParticle.transform.position, transform.position, Time.deltaTime * 100);
        }
    }

    void ReturnToPlayer()
    {
        if (GoToPlayer)
        {
            foreach (GameObject part in HookToObject.HookToParts)
            {
                if(part)
                    part.transform.position = Vector3.MoveTowards(part.transform.position, HookToObject.player.transform.position, Time.deltaTime * 20);

            }
        }
    }

    private void FixedUpdate()
    {
        HookToObject.DistanceToPlayer = Vector3.Distance(transform.position, HookToObject.player.transform.position);
        TakeParticle();
        CheckingStability();
        ReturnToPlayer();
    }
}
