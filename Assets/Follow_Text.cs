using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Follow_Text : MonoBehaviour {

    public GameObject player;
    public GameObject cam;
    public float distance_needed = 5.0f;

    public Input_Module input_m;
    public UI_Handler ui_handler;

    private bool in_range;
    private bool is_talking;
    private AudioSource audio_victory;

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        
        in_range = false;
        is_talking = false;

        audio_victory = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Vector3.Magnitude(player.transform.position - transform.position) < distance_needed)
            in_range = true;
        else
            in_range = false;


        if (in_range && !is_talking)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;

            Vector3 pos_to_camera = cam.transform.position - transform.position;
            gameObject.transform.LookAt(transform.position - pos_to_camera);

            if (input_m.is_talk_button_pressed())
            {
                is_talking = true;
                Game_Control.control.is_talking = true;
                Time.timeScale = 0; //pause time
                //open up talking guy
                SceneManager.LoadScene("dialog_window", LoadSceneMode.Additive);
            }
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        if (is_talking)
        {
            if (!Game_Control.control.is_talking)
            {
                is_talking = false;
                SceneManager.UnloadScene("dialog_window");
                Time.timeScale = 1; //restart time

                //if we won, show victory text
                if (Game_Control.control.get_save_data().cats_collected >= SaveData.total_cats)
                {
                    ui_handler.show_victory_text();
                    audio_victory.Play();
                }

                Game_Control.control.get_save_data().never_talked_to_granny = false;
                Game_Control.control.Save();
            }
        }
	}

    public bool is_in_range() { return in_range; }
}
