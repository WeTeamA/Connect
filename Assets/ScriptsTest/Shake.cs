using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public Vector3 GetTargetPosition(float time, float amplitude, float frequency)
    {
        Vector3 point = new Vector3();
        point.x = amplitude * Mathf.Cos(frequency * time);
        point.y = amplitude * Mathf.Sin(frequency * time);
        point.z = amplitude * Mathf.Sin(frequency * time + Mathf.PI / 4);
        return point;
    }
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += GetTargetPosition(Time.time, 0.003f * GameObject.Find("Player").GetComponent<Player>().playerStats.CurrentSpeed, 50);
    }
}
