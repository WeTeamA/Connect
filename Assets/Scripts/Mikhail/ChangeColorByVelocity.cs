using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorByVelocity : MonoBehaviour
{
    public Material ballMat;
    public List<Color> colors = new List<Color>();
    public float velocity;
    public float intencity;
    private float prevIntencity;
    private List<Color> fullColorList = new List<Color>();

    // Start is called before the first frame update
    void Start()
    {
        prevIntencity = intencity;
        for (int i = 0; i < colors.Count - 1; i++)
        {
            fullColorList.AddRange(ColorInterpolate(colors[i], colors[i + 1], 20));
        }
    }


    public List<Color> ColorInterpolate(Color colorOne, Color colorTwo, int iterations)
    {
        List<Color> colorList = new List<Color>();
        //Fill the colors List
        List<float> colorDiffs = new List<float>();
        colorDiffs.Add(colorTwo.r - colorOne.r);
        colorDiffs.Add(colorTwo.g - colorOne.g);
        colorDiffs.Add(colorTwo.b - colorOne.b);

        for (int i = 0; i < iterations; i++)
        {
            colorList.Add(new Color(
                                    (colorOne.r + (colorDiffs[0] / iterations) * i),
                                    (colorOne.g + (colorDiffs[1] / iterations) * i),
                                    (colorOne.b + (colorDiffs[2] / iterations) * i)
                                    ));
        }
        return colorList;
    }

    //Сигмоида
    public float Sigmoid(float x)
    {
        if (x < 0)
        {
            return 0;
        } else
        {
            return 2 * (1 / (1 + Mathf.Exp(-x))) - 1;
        }
    }

    //примерный диапазон vel = (0;6)
    public Color GetColorByVelocity(float vel, List<Color> fullColorList)
    {
        int index = (int)(Sigmoid(vel) * fullColorList.Count);
        Color resultColor = fullColorList[index];
        return resultColor;
    }

    // Update is called once per frame
    void Update()
    {
        List<Color> newFullColorList = new List<Color>();
        //замена всех цветов в fullColorList при изменении intenticy от кадра к кадру

        Color color = GetColorByVelocity(velocity, fullColorList);
        ballMat.SetColor("_EmissionColor", new Color(color.r*intencity, color.g*intencity, color.b*intencity));
    }
}
