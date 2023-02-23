using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometMovementScript : MonoBehaviour
{
    Vector2 meteor_speed = new Vector2(0, 0);

    void Start()
    {
        meteor_speed.y = -13; /* * Random.Range(10, 15);*/

        GetComponent<Rigidbody2D>().velocity = meteor_speed;

        transform.localScale *= Random.Range(1f, 2f);

        Quaternion rotation_b = Quaternion.AngleAxis(57, new Vector3(0, 0, -1));
        transform.rotation *= rotation_b;
    }

    void Update()
    {
        if (GetComponent<Rigidbody2D>().position.y < -15)
        {
            Destroy(GetComponent<Rigidbody2D>().gameObject);
        }
    }
}
