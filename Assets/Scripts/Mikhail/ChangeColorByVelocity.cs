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
    public static float Sigmoid(float x)
    {
            return 1 / (1 + Mathf.Exp(-x));
    }

    //примерный диапазон vel = (0;6)
    public Color GetColorByVelocity(float vel, List<Color> fullColorList)    {
        int index = (int)((2*Sigmoid(vel)-1) * fullColorList.Count);
        Color resultColor = fullColorList[index];
        return resultColor;
    }

    // Update is called once per frame
    void Update()
    {
        List<Color> newFullColorList = new List<Color>();
        //замена всех цветов в fullColorList при изменении intenticy от кадра к кадру
        velocity = GameObject.Find("Player").GetComponent<Player>().playerStats.CurrentSpeed*0.6f;
        intencity = 10+Sigmoid(1.35f*velocity-5) * 10;
        //intencity =1+ velocity * velocity*0.625f;

        Color color = GetColorByVelocity(velocity, fullColorList);
        ballMat.SetColor("_EmissionColor", new Color(color.r*intencity, color.g*intencity, color.b*intencity));
    }
}
