using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public bool showDirection;
    
    GameObject player;
    GameObject target;

    public GameObject directionSpherePrefab;
    public float setDecayInteisityDistance;
    Plane[] planes; //Ограничивающие плоскости камеры
    float distance; // Расстояние от снаряда до ограничевающей плоскости в координатах YZ
    float distancceMin; // Переменная для установления наименьшего a
    Vector3 direction; // Направление от снаряда до цели
    GameObject directionSphere; //Обоъект для создания шаблона префаба
    Material directionSphereMaterial; //Материал сферы направления
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = GameObject.FindGameObjectWithTag("Target");
        directionSphere = Instantiate(directionSpherePrefab); //Создаем шаблон префаба в игре
        directionSphereMaterial = directionSphere.GetComponent<MeshRenderer>().materials[0];
        directionSphere.SetActive(false);
    }

    Vector3 LastPos;

    // Update is called once per frame
    void Update()
    {
        if (player && LastPos != Vector3.zero) //Код для привязывания камеры к снаряду (player)
        {
            LastPos -= player.transform.position;
            transform.position -= LastPos;
        }
        LastPos = player.transform.position;

        SetDirection();    
    }

    public void SetDirection() //Метод, отвечающий за создание направляющей сферы до цели
    {
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main); //Заполняем массив плоскостей камеры

        if (!GeometryUtility.TestPlanesAABB(planes, target.GetComponent<SphereCollider>().bounds) && GeometryUtility.TestPlanesAABB(planes, player.GetComponent<SphereCollider>().bounds) && showDirection) //Если цель не находится в зоне видимости (ОПИРАЕМСЯ НА КОЛЛАЙДЕР ЦЕЛИ)
        {
            directionSphere.SetActive(true);

            direction = target.transform.position - player.transform.position; //Находим направление от снаряда к цели
            Ray ray = new Ray(player.transform.position, direction); //Создаем луч от снаряда к цели

            for (int i = 0; i <= 3; i++) //Наиходим наименьшую дистанцию между снарядом и точкой пересечения направления с любой из 4-х плоскостей
            {
                planes[i].Raycast(ray, out distance);
                if (distance < distancceMin && distance > 0) //Если дистанция до другой плоскости меньше, чем до заранее найденной - выбираем его
                    distancceMin = distance + 0.0185f; // 0.0185 - это подобранное значение, на которое сфера сдвигается от снаряда за рамку камеры
            }

            directionSphere.transform.position = direction.normalized * distancceMin + player.transform.position; //Перемещаем сферу направления в точку пересечения с ближайшей плоскостью (не забывая, что со сдвигом в 0,0185) 
            directionSphere.transform.localScale = new Vector3(Camera.main.transform.position.x / 7, Camera.main.transform.position.x / 7, Camera.main.transform.position.x / 7) * 0.05f; // Деление на 7 - это подобранный параметр, т.к. все настраивалось относительно положения камеры от плоскости YZ на расстоянии 7, а умножение на 0,05 - это возврат к базовому размеру шарика, тоже подобранный параметр
            // Строчка выше также нужна, чтобы размер directionSphere менялся в зависимости от удаления камеры

            directionSphereMaterial.SetColor("_GlowColor", target.GetComponent<MeshRenderer>().materials[0].GetColor("_GlowColor") * 6 * ChangeColorByVelocity.Sigmoid((direction.magnitude - setDecayInteisityDistance) * 10)); //Изменение свечения материала сферы направления по измененной сигмоиде. Функция сигмоиды определена в самописном скрипте. Умножение на 6 нужно для того, чтобы компенсировать слабое сияние цвета материала цели

            distancceMin = 10000000; //Устанавливаем немозможное большое значение, чтобы со следующего кадра все было ок
        }
        else
            directionSphere.SetActive(false); // Если цель видна - то выключаем подсветку
    }

}

