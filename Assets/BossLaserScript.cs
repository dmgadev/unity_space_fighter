using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaserScript : MonoBehaviour
{
    public Rigidbody2D laser;
    public CapsuleCollider2D laser_collider;
    float spawn_time;

    public BossShipScript boss_ship_script123;

    void Start()
    {
        spawn_time = Time.time;
        //boss_ship_script = boss_ref.gameObject.GetComponent<BossShipScript>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile_destroyer" || collision.gameObject.tag == "player_ship" || collision.gameObject.tag == "player_shield")
        {
            laser_collider.enabled = false;
        }
    }

    void Update()
    {
        if (boss_ship_script123.current_health <= 0)
        {
            //print(boss_ship_script.current_health + "|-here\n");
            Destroy(gameObject);
        }

        if (Time.time - spawn_time > 1.369f)
            Destroy(gameObject);
    }
}
