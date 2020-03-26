using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorByVelocity : MonoBehaviour
{
    public Material ballMat;
    public List<Color> colors = new List<Color>();
    public float velocity;
    public float intencity;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    /*
    public Color SetColor(float Velocity)
    { 
    
    }
    */

    public List<Color> ColorInterpolate(Color colorOne, Color colorTwo, int iterations, float intencity)
    {
        List<Color> colorList = new List<Color>();
        //Fill the colors List
        List<float> colorDiffs = new List<float>();
        colorDiffs.Add(colorTwo.r - colorOne.r);
        colorDiffs.Add(colorTwo.g - colorOne.g);
        colorDiffs.Add(colorTwo.b - colorOne.b);

        for (int i = 0; i < iterations; i++)
        {
            colorList.Add(new Color((colorOne.r + (colorDiffs[0] / iterations) * i)* intencity,
                                       (colorOne.g + (colorDiffs[1] / iterations) * i)* intencity,
                                       (colorOne.b + (colorDiffs[2] / iterations) * i)* intencity));
        }
        return colorList;
    }

    //Сигмоида
    public float Sigmoid(float x)
    {
        return 2*(1 / (1 + Mathf.Exp(-x)))-1;
    }

    //примерный диапазон vel = (0;6)
    public Color GetColorByVelocity(float vel, List<Color> colorList, float intencity)
    {
        List<Color> fullColorList = new List<Color>();
        for (int i = 0; i < colorList.Count - 1; i++)
        {
            fullColorList.AddRange(ColorInterpolate(colorList[i], colorList[i + 1], 100, intencity));
        }

        int colorsCount = fullColorList.Count;
        int index = (int)(Sigmoid(vel) * colorsCount);
        Color resultColor = fullColorList[index];
        return resultColor;
    }

    // Update is called once per frame
    void Update()
    {
        ballMat.SetColor("_EmissionColor", GetColorByVelocity(velocity, colors, intencity));
    }
}
