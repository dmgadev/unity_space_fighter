using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Rigidbody2D player_ship;
    public float Speed_Player;
    public float V_Speed_Player;

    public GameObject left_turn;
    public GameObject right_turn;
    public GameObject no_turn;

    public GameObject rocket;
    public GameObject default_projectile;

    public GameObject left_heart;
    public GameObject middle_heart;
    public GameObject right_heart;

    public AudioSource shot_taken;

    Quaternion projectile_rotation;
    Vector3 projectile_spawn;

    public SpriteMask rocket_mask; // x => -2.28f 100% | -2.7f 0%
    float offset_total = -2.28f + 2.7f;
    float rocket_cooldown;
    bool rocket_launched = false;

    public int remaining_player_health = 3;

    public bool shot_taken_flag = false; // на 2 секунды после пропущенного выстрела будем давать неуязвимость
    public float last_taken_shot_time = 0f;
    float invulnerability_duration = 2f;

    public bool is_invulnerable = true;

    EnemySpawningScript spawner_script;

    void Start()
    {
        projectile_rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
        InvokeRepeating("CreateDefaultProjectile", 0.5f, 0.5f);

        spawner_script = GameObject.Find("EnemySpawner").GetComponent<EnemySpawningScript>();
    }

    void CreateDefaultProjectile()
    {
        projectile_spawn = new Vector3(player_ship.transform.position.x, player_ship.transform.position.y - 3.5f, 5);
        Instantiate(default_projectile, projectile_spawn, projectile_rotation);
    }

    void Update()
    {
        if (rocket_launched == true)
        {
            if (Time.time - rocket_cooldown < 10)
            {
                rocket_mask.transform.position = new Vector3(-2.7f + (offset_total * (Time.time - rocket_cooldown) / 10), 4.6f, 1);
            }
            else
            {
                rocket_launched = false;
            }
        }

        if (remaining_player_health == 2)
        {
            right_heart.SetActive(false);
        }

        if (remaining_player_health == 1)
        {
            middle_heart.SetActive(false);
        }

        if (remaining_player_health == 0)
        {
            left_heart.SetActive(false);
        }

        if (shot_taken_flag == true && Time.time - last_taken_shot_time >= invulnerability_duration)
        {
            shot_taken_flag = false;
            last_taken_shot_time = 0f;
        }

        Vector3 final_vector = new Vector3(0, 0, 0);

        if (Input.GetAxis("Horizontal") > 0)
        {
            if (player_ship.position.x > 5.75f)
                final_vector += new Vector3(0, 0, 0);
            else
                final_vector += new Vector3(Speed_Player, 0, 0);

            left_turn.SetActive(false);
            right_turn.SetActive(true);
            no_turn.SetActive(false);
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            if (player_ship.position.x < -5.75f)
                final_vector += new Vector3(0, 0, 0);
            else
                final_vector += new Vector3(-Speed_Player, 0, 0);

            left_turn.SetActive(true);
            right_turn.SetActive(false);
            no_turn.SetActive(false);
        }
        if (Input.GetAxis("Horizontal") == 0)
        {
            left_turn.SetActive(false);
            right_turn.SetActive(false);
            no_turn.SetActive(true);
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            if (player_ship.position.y > 2.5)
                final_vector += new Vector3(0, 0, 0);
            else
                final_vector += new Vector3(0, V_Speed_Player, 0);
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            if (player_ship.position.y < -0.5)
                final_vector += new Vector3(0, 0, 0);
            else
                final_vector += new Vector3(0, -V_Speed_Player, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && rocket_launched == false)
        {
            projectile_spawn = new Vector3(player_ship.transform.position.x, player_ship.transform.position.y - 3.5f, 5);
            Instantiate(rocket, projectile_spawn, projectile_rotation);

            rocket_launched = true;
            rocket_cooldown = Time.time;
            rocket_mask.transform.position = new Vector3(-2.7f, 4.6f, 1);
        }

        player_ship.velocity = final_vector;

        if (remaining_player_health < 0)
        {
            //Destroy(GetComponent<Rigidbody2D>().gameObject);
            spawner_script.go_to_the_menu = 2; // хп меньше 0 - проиграли
        }
    }
}