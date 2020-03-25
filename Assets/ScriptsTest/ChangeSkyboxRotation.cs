﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkyboxRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Camera.main.transform.position.z * 0.4f);
    }
}