using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject GlowSphere;
    Plane[] Planes;
    public float a;
    float minimum;
    public Vector3 direction;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    { 
        direction = GameObject.Find("Heart").transform.position - GameObject.Find("ParticleBall").transform.position;
        Ray ray = new Ray(GameObject.Find("ParticleBall").transform.position, direction);
       
        Planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        for (int i = 0; i <= 3; i++)
        {
            Planes[i].Raycast(ray, out a);
            if (a < minimum && a>0)
                minimum = a;
        }

        GlowSphere.transform.position = direction.normalized * minimum + GameObject.Find("ParticleBall").transform.position;
        minimum = 10000000;
        //GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), GlowSphere.GetComponent<SphereCollider>().bounds);
    }
}
