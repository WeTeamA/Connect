using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BallExplose : MonoBehaviour
{
    //public GameObject Snar;
    Vector3 LastFramePos;
    Vector3 PreFramePos;
    Vector3 ResVector;
    public ParticleSystem Explose;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        LastFramePos = transform.position;
        ResVector = LastFramePos - PreFramePos;
        ParticleSystem p = Instantiate(Explose);
        p.transform.position = collision.transform.position;

        if (ResVector != new Vector3(0, 0, 0))
        {
            p.transform.rotation = Quaternion.LookRotation(ResVector);
        }
        else
        {
            p.transform.rotation = gameObject.transform.rotation;
            p.transform.Rotate(0, 180, 0);
        }

        p.Play();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PreFramePos = transform.position; //Подготовка последней позиции, чтобы потом было с чем сравнить новую позицию
    }

    
    
}
