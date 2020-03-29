using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distruction : MonoBehaviour
{
    float t;
    Vector3 Corner;
    // Start is called before the first frame update
    void Start()
    {


    }


    // Update is called once per frame
    void Update()
    {
        Corner = Camera.main.transform.position;
        Corner += new Vector3(-2.01f, 1.43f, -1f); //Коррекция относительно камеры (камера не может быть под углом)

        if (gameObject.GetComponent<FixedJoint>() == null)
        {
            t += Time.deltaTime; //Считаем время после исчезновения FixedJoint
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_GlowColor", Color.white);
        }

        if (t > 3)
        { 
            gameObject.GetComponent<MeshCollider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().AddForce((Corner - gameObject.transform.position) * 0.1f);
            if (gameObject.transform.position.x > Camera.main.transform.position.x-0.2f || t>5)
                 {
                     gameObject.SetActive(false);
                //Тут дописать пульсацию UI счетчика очков
                 }
        }
    }
}
