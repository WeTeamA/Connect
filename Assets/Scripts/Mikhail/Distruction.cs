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
        // Corner = GameObject.FindWithTag("ScreenCorner").transform.position; //Определяем угол экрана
        Corner = Camera.main.transform.position;
        Corner += new Vector3(-0.167f,-1f,0.4934f); //Коррекция относительно камеры (камера не может быть под углом)
    }


    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<FixedJoint>() == null)
        {
            t += Time.deltaTime; //Считаем время после исчезновения FixedJoint
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_GlowColor", Color.white);
        }

        if (t > 3)
        { 
            gameObject.GetComponent<MeshCollider>().isTrigger = true;
            gameObject.GetComponent<Rigidbody>().AddForce((Corner - gameObject.transform.position) * 0.006f);
            if (gameObject.transform.position.y > Camera.main.transform.position.y-0.2f || t>5)
                 {
                     gameObject.SetActive(false);
                //Тут дописать пульсацию UI счетчика очков
                 }
        }

        

        //Следующий код надо будет удалить потом!!!!!!!
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-2,2), Random.Range(-2, 2), Random.Range(-2, 2)));
        }

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), Random.Range(-2, 2)));

            }
        }
    }
}
