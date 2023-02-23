using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRocketScript : MonoBehaviour // all player projectiles script - not only for the rocket
{
    public Rigidbody2D player_projectile;
    public int projectile_damage;
    void Start()
    {
        player_projectile.velocity = new Vector3(0, 4f, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile_destroyer" || collision.gameObject.tag == "shield")
        {
            Destroy(GetComponent<Rigidbody2D>().gameObject);
        }

        if (collision.gameObject.tag == "enemy")
        {
            EnemyShipScript enemy_ship_ref = collision.gameObject.GetComponent<EnemyShipScript>();
            enemy_ship_ref.current_health -= projectile_damage;

            Destroy(GetComponent<Rigidbody2D>().gameObject);
        }
        if (collision.gameObject.tag == "boss")
        {
            BossShipScript enemy_ship_ref = collision.gameObject.GetComponent<BossShipScript>();
            enemy_ship_ref.current_health -= projectile_damage/2;

            Destroy(GetComponent<Rigidbody2D>().gameObject);
        }
    }

    void Update()
    {
        
    }
}
