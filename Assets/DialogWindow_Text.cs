using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogWindow_Text : MonoBehaviour {

    private int string_count;
    private int char_count;
    private AudioSource audio_blip;

	// Use this for initialization
	void Start () {
        audio_blip = GetComponent<AudioSource>();
        audio_blip.Stop();
        string_count = 0;
        char_count = 0;

        if (Game_Control.control.get_save_data().never_talked_to_granny)
        {
            Game_Control.control.current_dialog = Game_Control.control.dialog1;
        }
        else if (Game_Control.control.get_save_data().cats_collected < 3)
        {
            Game_Control.control.current_dialog = Game_Control.control.dialog2;
        }
        else if (Game_Control.control.get_save_data().cats_collected < 5)
        {
            Game_Control.control.current_dialog = Game_Control.control.dialog3;
        }
        else 
        {
            Game_Control.control.current_dialog = Game_Control.control.dialog4;
        }


	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.Joystick1Button1)) //THIS IS BAD and should be changed
        {
            //pressed B
            char_count = 0;
            string_count++;
        }
        else
            char_count++;

        update_text();
	}

    private void update_text()
    {
        if (string_count >= Game_Control.control.current_dialog.Length)
        {
            //done here
            Game_Control.control.is_talking = false;
            audio_blip.Stop();
            audio_blip.loop = false;
            return;
        }

        string curr_word = Game_Control.control.current_dialog[string_count];
        

        if (char_count > curr_word.Length)
        {
            char_count = curr_word.Length;
            audio_blip.Stop();
        }
        else
        {
            audio_blip.Play(); audio_blip.loop = true;
            audio_blip.pitch = Random.Range(0.5f, 1.5f);
        }
        string to_show = curr_word.Substring(0, char_count);
        gameObject.GetComponent<Text>().text = to_show;
    }
}
