using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class EnemyMovementScript : MonoBehaviour
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

    void Start()
    {
        pattern_name = new string[] { "SingleShot", "TripleShot", "MultipleShot" };
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
            if (pattern_is_set == false)
            {
                rand_pattern_gen = Random.Range(0, 3);
                pattern_is_set = true;

                // тайминги повторения паттернов выстрелов будут 2*1 для одиночного, 2*2 для тройного и 2*3 сек для мультивыстрела
                switch (rand_pattern_gen)
                {
                    case 0: InvokeRepeating(pattern_name[rand_pattern_gen], 2f, (rand_pattern_gen + 1) * 1.5f); break; // single shot
                    case 1: InvokeRepeating(pattern_name[rand_pattern_gen], 2f, (rand_pattern_gen + 1) * 1.5f); break; // triple shot
                    case 2: InvokeRepeating(pattern_name[rand_pattern_gen], 2f, (rand_pattern_gen + 1) * 1.5f); break; // multiple shot
                    default: break;
                }
                
            }
        }
    }

    private void stop_moving()
    {
        if (enemy_ship_rigid2d.velocity.x * enemy_ship_rigid2d.velocity.x + enemy_ship_rigid2d.velocity.y * enemy_ship_rigid2d.velocity.y <= 0.1f)
        {
            enemy_ship_rigid2d.velocity = new Vector2(0,0);
            is_in_final_location = true;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);

            InvokeRepeating("CancelAllShootingPatterns", 0f, 10.5f); // каждые 12 секунд обнуляем паттерн, чтобы

            //InvokeRepeating("CreateEnemyProjectile", 1f, 1f); // обычный выстрел в цель
        }
    }

    private void CancelAllShootingPatterns()
    {
        pattern_is_set = false;
        if (rand_pattern_gen != -1)
        {
            CancelInvoke(pattern_name[rand_pattern_gen]);
        }
    }

    private void SingleShot()
    {
        //--------------
        Vector3 enemy_projectile_spawn = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 5);
        //--------------

        // пытаемся написать указание направления вражеским снарядам
        GameObject mityan_ship = GameObject.Find("Turning_right_ship"); // переделать на нахождение угла поворота - скорость должна быть постоянной

        // вычисляем вектор, в направлении которого задавать скорость снаряду, затем умножаем его на разницу между вектором 
        // и стандартной скоростью снаряда(тем самым уменьшая скорость снаряда, который летит не по прямой)
        calculating_direction = new Vector2(mityan_ship.transform.position.x - enemy_projectile_spawn.x, mityan_ship.transform.position.y - enemy_projectile_spawn.y - 3.5f);
        velocity_difference = Mathf.Sqrt(projectile_default_velocity_module * projectile_default_velocity_module / (calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

        turn_angle = 180 / Mathf.PI * Mathf.Acos((calculating_direction.y*turn_vector.y) / Mathf.Sqrt(calculating_direction.x * calculating_direction.x + calculating_direction.y * calculating_direction.y));

        if (calculating_direction.x < 0) // корректировка поворота вектора в зависимости от четверти
            turn_angle *= -1;

        Quaternion enemy_projectile_rotation = Quaternion.AngleAxis(180+turn_angle, new Vector3(0, 0, 1));
        GameObject mityan = Instantiate(enemy_projectile_in_movement_script, enemy_projectile_spawn, enemy_projectile_rotation);
        
        EnemyProjectileScript mityan_script = mityan.GetComponent<EnemyProjectileScript>();
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
