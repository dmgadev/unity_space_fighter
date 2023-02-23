using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBaseScript : MonoBehaviour
{
    float spawn_time;
    bool laser_spawned = false;
    public GameObject laser_prefab;
    BossShipScript boss_ship_script;
    public GameObject boss_ref;

    GameObject laser_prefab_ref = null;

    void Start()
    {
        spawn_time = Time.time;
        boss_ship_script = boss_ref.gameObject.GetComponent<BossShipScript>();
        //print(boss_ship_script.current_health + "|-there\n");
    }

    void FixedUpdate()
    {
        
    }
    void Update()
    {
        if (boss_ship_script.current_health <= 0)
        {
            //print(boss_ship_script.current_health + "|-here\n");
            Destroy(gameObject);
            if (laser_prefab_ref != null)
            {
                Destroy(laser_prefab_ref.gameObject);
            }
        }

        if (Time.time - spawn_time > 2.664f && laser_spawned == false)
        {
            Vector3 spawn_laser_coords = new Vector3(gameObject.transform.position.x - 0.01f, gameObject.transform.position.y - 4.83f, gameObject.transform.position.z);
            Quaternion enemy_projectile_rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
            laser_prefab_ref = Instantiate(laser_prefab, spawn_laser_coords, enemy_projectile_rotation);
            laser_spawned = true;

            BossLaserScript bls_1 = laser_prefab_ref.gameObject.GetComponent<BossLaserScript>();
            bls_1.boss_ship_script123 = boss_ship_script;

        }
        if (Time.time - spawn_time > (2.664f + 1.369f))
        {
            Destroy(gameObject);
        }
        
    }
}
