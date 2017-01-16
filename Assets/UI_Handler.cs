using UnityEngine;
using System.Collections;

public class UI_Handler : MonoBehaviour {

    public Texture cat_image;

    private int save_counter;
    private static int save_counter_max = 50;

	// Use this for initialization
	void Start () {
        save_counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (save_counter > 0) save_counter--;
	}

    //temporary GUI labels to show data
    void OnGUI()
    {
        SaveData sd = Game_Control.control.get_save_data();

        GUI.Label(new Rect(10, 10, 100, 30), "HEALTH: " + sd.player_health + " / " + sd.player_max);
        GUI.Label(new Rect(50, 30, 100, 30), sd.cats_collected + " / " + sd.cats_available);
        GUI.Label(new Rect(10, 30, 30, 30), cat_image);

        if (save_counter > 0)
        {
            GUI.Label(new Rect(10, 50, 100, 30), "SAVED");
        }
        if (Game_Control.control.get_save_data().player_health == 0)
        {
            GUI.Label(new Rect(10, 70, 100, 30), "YOU DIED");
        }
    }

    public void show_save_text()
    {
        save_counter = save_counter_max;
    }
}
