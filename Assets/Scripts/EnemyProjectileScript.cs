using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    public Rigidbody2D enemy_projectile;
    public Vector2 projectile_velocity;

    void Start()
    {
        enemy_projectile.velocity = projectile_velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile_destroyer" || collision.gameObject.tag == "player_ship" || collision.gameObject.tag == "player_shield")
        {
            Destroy(GetComponent<Rigidbody2D>().gameObject);
        }
    }

    void Update()
    {

    }
}

