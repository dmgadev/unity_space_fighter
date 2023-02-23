using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenerationScript : MonoBehaviour
{
    public GameObject meteor1;
    public GameObject meteor2;
    public GameObject meteor3;
    public GameObject meteorcomet;

    Quaternion rotation_b = Quaternion.AngleAxis(0, new Vector3(0, 0, -1));

    Vector3 tr;

    float rand_x;
    int rand_y, spawn_choice;
    int local_buff;

    void Start()
    {
        InvokeRepeating("AddGameObject", 0f, 0.5f);
    }

    void AddGameObject()
    {
        tr = new Vector3(0, 12, 50);
        rand_x = Random.Range(-5f, 5f);
        rand_y = Random.Range(-2, 3);
        tr.x += rand_x;
        tr.y += rand_y;
        spawn_choice = Random.Range(0, 4);

        if (spawn_choice == 0)
        {
            Instantiate(meteor1, tr, rotation_b);
        }
        if (spawn_choice == 1)
        {
            Instantiate(meteor2, tr, rotation_b);
        }
        if (spawn_choice == 2)
        {
            Instantiate(meteor3, tr, rotation_b);
        }
        if (spawn_choice == 3)
        {
            local_buff = Random.Range(0, 6);
            if (local_buff < 1)
                Instantiate(meteorcomet, tr, rotation_b);
        }
    }

    void Update()
    {

    }

}
