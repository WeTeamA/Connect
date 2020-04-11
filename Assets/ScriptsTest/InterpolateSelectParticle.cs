using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateSelectParticle : MonoBehaviour
{
    // Start is called before the first frame update
    public bool IsClosest;

    ParticleSystem.ShapeModule pShape;
    
    void Start()
    {
        pShape = GameObject.Find("SelectParticle").GetComponent<ParticleSystem>().shape;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClosest)
        {
            if (pShape.radius <= 2)
                pShape.radius +=Time.deltaTime*10;
        }
        else
        {
            pShape.radius -= Time.deltaTime * 10;
        }
    }
}
