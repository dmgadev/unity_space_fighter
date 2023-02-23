using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldScript : MonoBehaviour
{
    float time_of_collision = 0;
    bool collision_flag = false;

    PlayerMovementScript parent_script_ref;

    public Animator player_shield;
    public RuntimeAnimatorController player_shield_animation;
    public RuntimeAnimatorController player_shield_animation_idle;
    public RuntimeAnimatorController player_shield_animation_growing;
    public SpriteRenderer sprite_renderer;

    Vector3 scale_vector;

    bool first_f = false;
    bool second_f = false;
    bool third_f = false;

    bool scaling_started = false; // за 5 сек надо отскалировать текстуру на 0.3 (0.3 = 100% масштаба)

    void Start()
    {
        scale_vector = new Vector3(0.01f, 0.01f, 1);
        parent_script_ref = GameObject.Find("Turning_right_ship").GetComponent<PlayerMovementScript>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy_projectiles" && collision_flag == false)
        {
            time_of_collision = Time.time;
            collision_flag = true;
            player_shield.runtimeAnimatorController = player_shield_animation;
            first_f = false;
            second_f = false;
            third_f = false;
        }
    }

    void Update()
    {
        if (collision_flag == true)
        {
            if (Time.time - time_of_collision >= 1.5f && first_f == false)
            {
                parent_script_ref.is_invulnerable = false;

                gameObject.GetComponent<CircleCollider2D>().enabled = false;
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                sprite_renderer.sortingOrder = 1;

                first_f = true;
            }
            if (Time.time - time_of_collision >= 5f && second_f == false)
            {
                player_shield.runtimeAnimatorController = player_shield_animation_growing;
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                
                gameObject.transform.localScale = scale_vector;

                scaling_started = true;
                second_f = true;
            }

            if (Time.time - time_of_collision >= 10f && third_f == false)
            {
                gameObject.GetComponent<CircleCollider2D>().enabled = true;
                gameObject.transform.localScale = new Vector3(0.31f, 0.31f, 1);
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
                player_shield.runtimeAnimatorController = player_shield_animation_idle;

                scale_vector = new Vector3(0.01f, 0.01f, 1);
                collision_flag = false;

                third_f = true;
                scaling_started = false;

                parent_script_ref.is_invulnerable = true;
            }
            if (scaling_started == true)
            {
                scale_vector.x += Time.deltaTime * 0.3f / 5;
                scale_vector.y += Time.deltaTime * 0.3f / 5;
                gameObject.transform.localScale = scale_vector;
            }
        }
    }
}
