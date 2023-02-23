using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossShipScript : MonoBehaviour
{
    public int current_health = 100;
    Vector2 target_spawn_pos;

    public Rigidbody2D enemy_ship_rigid2d;

    public AudioSource exp;
    Time destroy_capture_time;
    float time_test;
    bool time_captured = false;

    public GameObject blow;

    public Vector2 final_location;

    GameObject prefab1234;

    public AudioSource hit_sound;

    EnemySpawningScript spawner_script;

    public AudioSource boss_music;

    Text boss_health_label;
    SpriteMask boss_health_mask; // x = 4.7 -> x = 2.145
    float mask_total_offset = 4.7f - 2.145f;
    float mask_default_pos_x = 4.7f;


    void Start()
    {
        spawner_script = GameObject.Find("EnemySpawner").GetComponent<EnemySpawningScript>();


        boss_health_label = GameObject.Find("boss_health_number").GetComponent<Text>();
        boss_health_mask = GameObject.Find("boss_health_mask").GetComponent<SpriteMask>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "player_projectiles")
        {
            hit_sound.Play();
        }
    }

    void Update()
    {
        if (current_health < 0)
        {
            current_health = 0;
        }
        boss_health_label.text = "" + current_health;
        boss_health_mask.transform.position = new Vector3(4.7f - 2.56888f + mask_default_pos_x - (mask_total_offset * (100 - current_health) / 100), 4.395109f + 0.22f, 1);
        //print(boss_health_mask.transform.position.z + "|");
        if (current_health <= 0 && time_captured == true)
        {
            if ((Time.time - time_test) >= exp.clip.length)
            {
                Destroy(GetComponent<Rigidbody2D>().gameObject);
                Destroy(prefab1234.gameObject);

                spawner_script.current_alive_enemies--; // при уничтожении объекта уменьшаем кол-во живых врагов
                spawner_script.enemies_alive--;

                // Загрузим меню
                spawner_script.go_to_the_menu = 1; // убили босса = выиграли
            }
        }

        if (current_health <= 0 && time_captured == false)
        {
            // остановить спавн снарядов
            BossMovementScript enemy_ship_movement_script_ref = gameObject.GetComponent<BossMovementScript>();
            enemy_ship_movement_script_ref.StopInvocaton();
            boss_music.Stop();

            Vector3 buff1234 = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            Quaternion projectile_rotation1234 = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
            time_test = Time.time;
            prefab1234 = Instantiate(blow, buff1234, projectile_rotation1234);
            prefab1234.transform.localScale = new Vector3(2, 2, 2);

            time_captured = true;
            gameObject.GetComponent<Collider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
