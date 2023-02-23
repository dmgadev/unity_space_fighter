using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScript : MonoBehaviour
{
    InfoScript info_object_script_ref;
    GameObject info_object_ref;
    bool win;
    bool lose;

    public AudioSource win_audio;
    public AudioSource lose_audio;

    public GameObject button;
    public GameObject win_t;
    public GameObject lose_t;
    public GameObject control_t;
    public GameObject restart_t;

    public GameObject player_ship_model;

    private void Awake()
    {
        
    }

    void Start()
    {
        info_object_ref = GameObject.Find("InfoObject");

        if (info_object_ref != null)
        {
            info_object_script_ref = info_object_ref.GetComponent<InfoScript>();

            win = info_object_script_ref.win;
            lose = info_object_script_ref.lose;

            if (win == true)
            {
                button.SetActive(false);
                win_t.SetActive(true);
                lose_t.SetActive(false);
                control_t.SetActive(false);
                restart_t.SetActive(true);
                player_ship_model.SetActive(false);
            }

            if (lose == true)
            {
                button.SetActive(false);
                win_t.SetActive(false);
                lose_t.SetActive(true);
                control_t.SetActive(false);
                restart_t.SetActive(true);
                player_ship_model.SetActive(false);
            }

            Destroy(info_object_ref);
        }

        if (win == true)
        {
            win_audio.Play();
        }
        if (lose == true)
        {
            lose_audio.Play();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && (win != false || lose != false))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
