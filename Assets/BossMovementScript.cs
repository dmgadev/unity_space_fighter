using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovementScript : MonoBehaviour
{
    public Vector2 spawn_location; // из другого скрипта задаем точку спавна
    public Vector2 final_location; // и точку, в которую ему надо лететь и по достижении которой, корабль уже начнет действия

    bool is_in_final_location = false;

    public Rigidbody2D enemy_ship_rigid2d; // сам корабль, которым управляет скрипт
    public GameObject enemy_projectile_in_movement_script; // один из снарядов
    public GameObject enemy_projectile_circle_in_movement_script; // один из снарядов
    public GameObject enemy_projectile_blast_in_movement_script; // один из снарядов

    float projectile_default_velocity_module = 4f;

    Vector2 calculating_direction;
    float velocity_difference; // todo
    float turn_angle;

    Vector2 turn_vector = new Vector2(0, -1);

    Vector2 current_velocity;

    string[] pattern_name;
    bool pattern_is_set = false;
    int rand_pattern_gen = -1;

    float[] stop_x_coords = new float[] { -4, 0, 4 }; // X координаты, между которыми будет курсировать босс. Y всегда 2 для него

    float position_reach_time = 0;
    int current_position_selected = -1;

    public GameObject laser_base;

    BossShipScript boss_ship_script;

    void Start()
    {
        pattern_name = new string[] { "SingleShot", "TripleShot", "MultipleShot"};
        boss_ship_script = gameObject.GetComponent<BossShipScript>();
    }

    void Update()
    {
        if (is_in_final_location == false)
        {
            current_velocity = new Vector2(final_location.x - gameObject.transform.position.x, final_location.y - gameObject.transform.position.y);
            enemy_ship_rigid2d.velocity = current_velocity;
            stop_moving();
        }
        else
        {
            if (current_position_selected != -1)
            {
                if (Mathf.Abs(gameObject.transform.position.x - stop_x_coords[current_position_selected]) < 0.3f)
                {
                    enemy_ship_rigid2d.velocity = new Vector2(0,0);
                    current_position_selected = -1;
                    position_reach_time = Time.time;
                }
            }
            else
            {
                if (Time.time - position_reach_time > 4.5f)
                {
                    current_position_selected = Random.Range(0, 3);
                    enemy_ship_rigid2d.velocity = new Vector2(stop_x_coords[current_position_selected] - gameObject.transform.position.x, 0);
                    //stop invokation maybe?
                    //---------------------
                    CancelAllShootingPatterns();
                    //---------------------
                }
                else
                {
                    // если мы только прилетели на одну из точек патрулирования, то выбираем паттерн стрельбы в коде ниже
                    if (pattern_is_set == false)
                    {
                        pattern_is_set = true;
                        rand_pattern_gen = Random.Range(0, 2);

                        // тайминги повторения паттернов выстрелов будут 2*1 для одиночного, 2*2 для тройного и 2*3 сек для мультивыстрела
                        switch (rand_pattern_gen) 
                        {
                            case 0: laser_shot(); break; //InvokeRepeating(pattern_name[1], 0f, (rand_pattern_gen + 1)); break; // triple shot
                            case 1: laser_shot(); break; // laser
                            default: break;
                        }

                    }
                }
            }
        }
    }

    private void laser_shot() // спавн спрайта кружка на -1.15 по Y относительно босса, а лазера по Х -0.01 и по Y -5.9
    {
        if (boss_ship_script.current_health > 0)
        {
            Vector3 spawn_laser_coords = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1.15f, gameObject.transform.position.z);
            Quaternion enemy_projectile_rotation = Quaternion.AngleAxis(180, new Vector3(0, 0, 1));
            GameObject big_laser_instance = Instantiate(laser_base, spawn_laser_coords, enemy_projectile_rotation);
            LaserBaseScript laser_base_script_ref = big_laser_instance.gameObject.GetComponent<LaserBaseScript>();
            laser_base_script_ref.boss_ref = gameObject;
        }
    }

    private void stop_moving()
    {
        if (enemy_ship_rigid2d.velocity.x * enemy_ship_rigid2d.velocity.x + enemy_ship_rigid2d.velocity.y * enemy_ship_rigid2d.velocity.y <= 0.1f)
        {
            enemy_ship_rigid2d.velocity = new Vector2(0, 0);
            is_in_final_location = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);

            //InvokeRepeating("CancelAllShootingPatterns", 0f, 10.5f); // каждые 10.5 секунд обнуляем паттерн

            InvokeRepeating(pattern_name[0], 1f, 3f);

            InvokeRepeating(pattern_name[2], 0f, 4f);

            //InvokeRepeating("CreateEnemyProjectile", 1f, 1f); // обычный выстрел в цель
        }
    }

    private void CancelAllShootingPatterns()
    {
        pattern_is_set = false;
        CancelInvoke(pattern_name[1]);
    }

    private void SingleShot() // 2 singles at the same time on boss
    {
        //--------------
        Vector3 enemy_projectile_spawn = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 5);
        //--------------

        // пытаемся написать указание направления вражеским снарядам
        GameObject mityan_ship = GameObject.Find("Turning_right_ship"); // переделать на нахождение угла поворота - скорость должна быть постоянной

        // вычисляем вектор, в направлении которого задавать скорость снаряду, затем умножаем его на разницу между вектором 
        // и стандартной скоростью снаряда(тем самым уменьшая скорость снаряда, который летит не по прямой)
        enemy_projectile_spawn.x += 0.75f;
        calculating_direction = new Vector2(mityan_ship.transform.position.x - (enemy_projectile_spawn.x), mityan_ship.transform.position.y - enemy_projectile_spawn.y - 3.5f);
        velocity_difference = Mathf.Sqrt(projectile_default_velocity_module * projectile_default_velocity_module / (calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

        turn_angle = 180 / Mathf.PI * Mathf.Acos((calculating_direction.y * turn_vector.y) / Mathf.Sqrt(calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

        if (calculating_direction.x < 0) // корректировка поворота вектора в зависимости от четверти
            turn_angle *= -1;

        Quaternion enemy_projectile_rotation = Quaternion.AngleAxis(180 + turn_angle, new Vector3(0, 0, 1));
        GameObject mityan = Instantiate(enemy_projectile_in_movement_script, enemy_projectile_spawn, enemy_projectile_rotation);

        EnemyProjectileScript mityan_script = mityan.GetComponent<EnemyProjectileScript>();
        mityan_script.projectile_velocity = calculating_direction * velocity_difference;

        //генерируем второй снаряд
        enemy_projectile_spawn.x -= 1.5f;
        calculating_direction = new Vector2(mityan_ship.transform.position.x - (enemy_projectile_spawn.x), mityan_ship.transform.position.y - enemy_projectile_spawn.y - 3.5f);
        velocity_difference = Mathf.Sqrt(projectile_default_velocity_module * projectile_default_velocity_module / (calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

        turn_angle = 180 / Mathf.PI * Mathf.Acos((calculating_direction.y * turn_vector.y) / Mathf.Sqrt(calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

        if (calculating_direction.x < 0) // корректировка поворота вектора в зависимости от четверти
            turn_angle *= -1;

        enemy_projectile_rotation = Quaternion.AngleAxis(180 + turn_angle, new Vector3(0, 0, 1));
        mityan = Instantiate(enemy_projectile_in_movement_script, enemy_projectile_spawn, enemy_projectile_rotation);

        mityan_script = mityan.GetComponent<EnemyProjectileScript>();
        mityan_script.projectile_velocity = calculating_direction * velocity_difference;
    }
    private void MultipleShot()
    {
        Vector3 enemy_projectile_spawn = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 5);

        float buff_a, buff_b;
        Vector2 buff_for_vector_calculation;
        float turn_angle_to_calc_point = 22.5f;

        for (int i = 0; i < 7; i++)
        {
            buff_a = Mathf.Cos(turn_angle_to_calc_point * (i + 1) * Mathf.PI / 180);
            buff_b = Mathf.Sin(turn_angle_to_calc_point * (i + 1) * Mathf.PI / 180);
            buff_for_vector_calculation.x = enemy_projectile_spawn.x + buff_a;
            buff_for_vector_calculation.y = enemy_projectile_spawn.y - buff_b;

            // вычисляем вектор, в направлении которого задавать скорость снаряду, затем умножаем его на разницу между вектором 
            // и стандартной скоростью снаряда(тем самым уменьшая скорость снаряда, который летит не по прямой)
            calculating_direction = new Vector2(buff_for_vector_calculation.x - enemy_projectile_spawn.x, buff_for_vector_calculation.y - enemy_projectile_spawn.y);
            velocity_difference = Mathf.Sqrt(0.25f * projectile_default_velocity_module * projectile_default_velocity_module / (calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));
            // 0.25f выше для поправки скорости шариков в 2 раза медленнее
            turn_angle = 180 / Mathf.PI * Mathf.Acos((calculating_direction.y * turn_vector.y) / Mathf.Sqrt(calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

            if (calculating_direction.x < 0) // корректировка поворота вектора в зависимости от четверти
                turn_angle *= -1;

            Quaternion enemy_projectile_rotation = Quaternion.AngleAxis(180 + turn_angle, new Vector3(0, 0, 1));
            GameObject mityan = Instantiate(enemy_projectile_circle_in_movement_script, enemy_projectile_spawn, enemy_projectile_rotation);

            EnemyProjectileScript mityan_script = mityan.GetComponent<EnemyProjectileScript>();
            mityan_script.projectile_velocity = calculating_direction * velocity_difference;
        }
    }

    private void TripleShot()
    {
        Vector3 enemy_projectile_spawn = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 5);

        float buff_a = 1, buff_b = 0;
        Vector2 buff_for_vector_calculation = new Vector2(enemy_projectile_spawn.x + buff_a, enemy_projectile_spawn.y + buff_b);
        float turn_angle_to_calc_point = 60;

        for (int i = 0; i < 3; i++)
        {

            buff_a = Mathf.Cos(turn_angle_to_calc_point * Mathf.PI / 180);
            buff_b = Mathf.Sin(turn_angle_to_calc_point * Mathf.PI / 180);
            buff_for_vector_calculation.x = enemy_projectile_spawn.x + buff_a;
            buff_for_vector_calculation.y = enemy_projectile_spawn.y - buff_b;
            turn_angle_to_calc_point += 30;
            // вычисляем вектор, в направлении которого задавать скорость снаряду, затем умножаем его на разницу между вектором 
            // и стандартной скоростью снаряда(тем самым уменьшая скорость снаряда, который летит не по прямой)
            calculating_direction = new Vector2(buff_for_vector_calculation.x - enemy_projectile_spawn.x, buff_for_vector_calculation.y - enemy_projectile_spawn.y);
            velocity_difference = Mathf.Sqrt(0.75f * 0.75f * projectile_default_velocity_module * projectile_default_velocity_module / (calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));
            // для поправки скорости грушевидных снарядов на 0.75 от обычной
            turn_angle = 180 / Mathf.PI * Mathf.Acos((calculating_direction.y * turn_vector.y) / Mathf.Sqrt(calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

            if (calculating_direction.x < 0) // корректировка поворота вектора в зависимости от четверти
                turn_angle *= -1;

            Quaternion enemy_projectile_rotation = Quaternion.AngleAxis(180 + turn_angle, new Vector3(0, 0, 1));
            GameObject mityan = Instantiate(enemy_projectile_blast_in_movement_script, enemy_projectile_spawn, enemy_projectile_rotation);

            EnemyProjectileScript mityan_script = mityan.GetComponent<EnemyProjectileScript>();
            mityan_script.projectile_velocity = calculating_direction * velocity_difference;
        }
    }

    public void StopInvocaton()
    {
        CancelInvoke();
    }
}
