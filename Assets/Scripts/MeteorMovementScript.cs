using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorMovementScript : MonoBehaviour
{
    Vector2 meteor_speed = new Vector2(0, 0);
    Quaternion rotation_b;
    int rotation_angle;

    void Start()
    {
        rotation_angle = Random.Range(1, 5);
        int rotation_module = Random.Range(-1, 1);

        if (rotation_module != 0)
        {
            rotation_angle *= rotation_module;
        }

        meteor_speed.y = -1 * Random.Range(6, 8);

        GetComponent<Rigidbody2D>().velocity = meteor_speed;

        transform.localScale *= Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        if (GetComponent<Rigidbody2D>().position.y < -15)
        {
            Destroy(GetComponent<Rigidbody2D>().gameObject);
        }

        rotation_b = Quaternion.AngleAxis(rotation_angle, new Vector3(0, 0, -1));
        transform.rotation *= rotation_b;
    }
}
