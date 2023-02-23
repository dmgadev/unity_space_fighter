using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipCollisionScript : MonoBehaviour
{
    public GameObject parent_ship;

    PlayerMovementScript parent_script_ref;

    Animator center_ship_hit;
    Animator right_ship_hit;
    Animator left_ship_hit;

    public RuntimeAnimatorController center_idle;
    public RuntimeAnimatorController center_damaged;

    public RuntimeAnimatorController left_idle;
    public RuntimeAnimatorController left_damaged;

    public RuntimeAnimatorController right_idle;
    public RuntimeAnimatorController right_damaged;



    void Start()
    {
        center_ship_hit = GameObject.Find("Turning_right_ship").transform.GetChild(0).gameObject.GetComponent<Animator>();
        right_ship_hit = GameObject.Find("Turning_right_ship").transform.GetChild(1).gameObject.GetComponent<Animator>();
        left_ship_hit = GameObject.Find("Turning_right_ship").transform.GetChild(2).gameObject.GetComponent<Animator>();

        //gameObject.transform.GetChild(0).gameObject.SetActive(false);

        parent_script_ref = parent_ship.GetComponent<PlayerMovementScript>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (parent_script_ref.is_invulnerable == false)
        {
            if (parent_script_ref.shot_taken_flag == false && collision.gameObject.tag == "enemy_projectiles")
            {
                parent_script_ref.shot_taken_flag = true; // устанавливаем флаг неуязвимости
                parent_script_ref.last_taken_shot_time = Time.time; // устанавливаем время столкновения

                parent_script_ref.remaining_player_health -= 1; // вычитаем хп

                parent_script_ref.shot_taken.Play();

                center_ship_hit.runtimeAnimatorController = center_damaged;
                right_ship_hit.runtimeAnimatorController = right_damaged;
                left_ship_hit.runtimeAnimatorController = left_damaged;
            }
        }
    }

    void Update()
    {
        if (parent_script_ref.shot_taken_flag == false)
        {
            center_ship_hit.runtimeAnimatorController = center_idle;
            right_ship_hit.runtimeAnimatorController = right_idle;
            left_ship_hit.runtimeAnimatorController = left_idle;
        }
    }
}
