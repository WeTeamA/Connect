using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
     [SerializeField] List<GameObject> Parts = new List<GameObject>();
     int Count;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform part in transform)
        {
            Parts.Add(part.gameObject);
        }
        Count = Parts.Count;
    }

    void CheckConnections()
    {
        foreach (GameObject part in Parts)
        {
            if (!part.GetComponent<FixedJoint>() && part.layer != 11)
            {
                StartCoroutine(DiscardPiece(part));

            }
        }
        
    }

    IEnumerator DiscardPiece(GameObject part)
    {
        part.layer = 11;

        Count--;

        if (Count == 0)
        {
            GameController.manager.EndLevel(false);
        }
        GameController.manager.IncreaseScore(50);


        part.GetComponent<MeshRenderer>().materials[0].SetColor("_GlowColor", Color.white);
        Vector3 Corner = Camera.main.transform.position;
        Corner += new Vector3(-2.01f, 1.43f, -1f); //Коррекция относительно камеры (камера не может быть под углом)


        yield return new WaitForSeconds(3);

        part.GetComponent<MeshCollider>().isTrigger = true;
        part.GetComponent<Rigidbody>().AddForce((Corner - part.transform.position) * 0.25f, ForceMode.Impulse);

        yield return new WaitForSeconds(1);

        part.SetActive(false);
        

    }

    // Update is called once per frame
    void Update()
    {
        CheckConnections();
    }
}
