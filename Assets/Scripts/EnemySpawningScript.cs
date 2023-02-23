using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemySpawningScript : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject enemy4;
    public GameObject enemy5;

    public GameObject boss_ship;

    GameObject[] enemies_array;

    Vector2[] possible_spawn_coords = { new Vector2(-15, 10), new Vector2(-4, 15), new Vector2(0, 15), new Vector2(4, 15), new Vector2(15, 10) };
    Vector2[] possible_final_coords_for3enemies = { new Vector2(-4.5f, 1.75f), new Vector2(0, 2.5f), new Vector2(4.5f, 1.75f) };
    Vector2[] possible_final_coords_for4enemies = { new Vector2(-4.5f, 1.75f), new Vector2(-2, 2.5f), new Vector2(2, 2.5f), new Vector2(4.5f, 1.75f) };

    public int enemies_alive = 5; // изменить на 20

    public int current_alive_enemies = 0;

    int rand_num;
    bool[] rand_order;
    bool[] rand_final_coords;
    bool boss_spawned = false;

    // переменные, используемые для спавна
    Quaternion spawn_rotation;
    EnemyMovementScript enemy_movement_script_ref;
    BossMovementScript boss_movement_script_ref;
    GameObject current_enemy;
    Vector3 instantiation_coords;

    public int go_to_the_menu = 0;

    public Text enemies_remaining_label;

    InfoScript info_script;

    void Start()
    {
        spawn_rotation = Quaternion.AngleAxis(180, new Vector3(0, 0, 1));
        enemies_array = new GameObject[] { enemy1, enemy2, enemy3, enemy4, enemy5 }; // объявляем массив врагов

        rand_num = Random.Range(3, 5);
        if (rand_num == 3)
            spawn_more_enemies3();
        else
            spawn_more_enemies4();

        info_script = GameObject.Find("InfoObject").GetComponent<InfoScript>();
    }

    void Update()
    {
        if (go_to_the_menu != 0)
        {
            if (go_to_the_menu == 1)
            {
                info_script.win = true;
                info_script.lose = false;
            }
            if (go_to_the_menu == 2)
            {
                info_script.win = false;
                info_script.lose = true;
            }
            SceneManager.LoadScene("MenuScene");
        }

        if (enemies_alive < 0)
        {
            enemies_alive = 0;
            
        }
        enemies_remaining_label.text = "" + enemies_alive;

        // когда enemies_alive <= 0 заспавнить босса
        if (enemies_alive <= 0 && current_alive_enemies == 0)
        {
            if (boss_spawned == false)
            {
                boss_spawned = true;
                instantiation_coords = new Vector3(possible_spawn_coords[2].x, possible_spawn_coords[2].y, 5);

                current_enemy = Instantiate(boss_ship, instantiation_coords, spawn_rotation); // выбрать босса в качестве объекта спавна
                boss_movement_script_ref = current_enemy.GetComponent<BossMovementScript> ();

                boss_movement_script_ref.spawn_location = possible_spawn_coords[2];
                boss_movement_script_ref.final_location = new Vector2(0, 2f);
            }
        }
        else
        {
            if (current_alive_enemies == 0)
            {
                rand_num = Random.Range(3, 5); // 3 or 4
                if (rand_num == 3)
                    spawn_more_enemies3();
                else
                    spawn_more_enemies4();
            }
        }
    }

    void spawn_more_enemies3()
    {
        current_alive_enemies = 3;

        rand_final_coords = new bool[] { false, false, false };

        rand_order = new bool[] { false, false, false, false, false };
        generate_order(3);

        bool final_value_set;
        int rand_num_in_spawn_f;
        for (int j = 0; j < 5; j++)
        {
            if (rand_order[j] == false)
                continue;

            for (int i = 0; i < 3; i++)
            {
                final_value_set = false;
                while (final_value_set == false)
                {
                    rand_num_in_spawn_f = Random.Range(0, 3);
                    if (rand_final_coords[rand_num_in_spawn_f] == false)
                    {
                        instantiation_coords = new Vector3(possible_spawn_coords[j].x, possible_spawn_coords[j].y, 5);

                        current_enemy = Instantiate(enemies_array[Random.Range(0, 5)], instantiation_coords, spawn_rotation);
                        enemy_movement_script_ref = current_enemy.GetComponent<EnemyMovementScript>();

                        enemy_movement_script_ref.spawn_location = possible_spawn_coords[j];
                        enemy_movement_script_ref.final_location = possible_final_coords_for3enemies[rand_num_in_spawn_f];

                        rand_final_coords[rand_num_in_spawn_f] = true;

                        final_value_set = true;
                        i = 3;
                    }
                }
            }
        }
    }

    void spawn_more_enemies4()
    {
        current_alive_enemies = 4;

        rand_final_coords = new bool[] { false, false, false, false };

        rand_order = new bool[] { false, false, false, false, false };
        generate_order(4);

        bool final_value_set;
        int rand_num_in_spawn_f;
        for (int j = 0; j < 5; j++)
        {
            if (rand_order[j] == false)
                continue;

            for (int i = 0; i < 4; i++)
            {
                final_value_set = false;
                while (final_value_set == false)
                {
                    rand_num_in_spawn_f = Random.Range(0, 4);
                    if (rand_final_coords[rand_num_in_spawn_f] == false)
                    {
                        instantiation_coords = new Vector3(possible_spawn_coords[j].x, possible_spawn_coords[j].y, 5);

                        current_enemy = Instantiate(enemies_array[Random.Range(0, 5)], instantiation_coords, spawn_rotation);
                        enemy_movement_script_ref = current_enemy.GetComponent<EnemyMovementScript>();

                        enemy_movement_script_ref.spawn_location = possible_spawn_coords[j];
                        enemy_movement_script_ref.final_location = possible_final_coords_for4enemies[rand_num_in_spawn_f];

                        rand_final_coords[rand_num_in_spawn_f] = true;

                        final_value_set = true;
                        i = 4;
                    }
                }
            }
        }
    }

    void generate_order(int quan)
    {
        bool value_set;
        int rand_num_in_f;
        for (int i = 0; i < quan; i++)
        {
            value_set = false;
            while (value_set == false)
            { 
                rand_num_in_f = Random.Range(0, 5);
                if (rand_order[rand_num_in_f] == false)
                {
                    rand_order[rand_num_in_f] = true;
                    value_set = true;
                }
            }
        }
    }
}
