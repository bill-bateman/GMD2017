using UnityEngine;
using System.Collections;

public class UI_Handler : MonoBehaviour {

    public Texture cat_image;
    public Texture background;

    private int save_counter;
    private static int save_counter_max = 50;

    private int jump_text_counter;
    private static int jump_text_counter_max = 100;

    private int victory_text_counter;
    private static int victory_text_counter_max = 200;

	// Use this for initialization
	void Start () {
        save_counter = 0;
        jump_text_counter = 0;
        victory_text_counter = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (save_counter > 0) save_counter--;
        if (jump_text_counter > 0) jump_text_counter--;
        if (victory_text_counter > 0) victory_text_counter--;
	}

    //temporary GUI labels to show data
    void OnGUI()
    {
        GUIStyle large_text = new GUIStyle(GUI.skin.label);
        large_text.fontSize = 25;

        SaveData sd = Game_Control.control.get_save_data();

        GUI.Label(new Rect(10, 10, 200, 30), "HEALTH: " + sd.player_health + " / " + sd.player_max, large_text);
        GUI.Label(new Rect(60, 40, 200, 30), sd.cats_collected + "", large_text);
        GUI.DrawTexture(new Rect(10, 40, 30, 30), cat_image);

        int w = Screen.width, h = Screen.height;
        int mid_w = (w / 2), mid_h = (h / 2);

        if (save_counter > 0)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.black);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(new Rect(mid_w - 50, mid_h + 100, 90, 30), GUIContent.none);

            GUI.Label(new Rect(mid_w-50, mid_h+100, 100, 30), "SAVED", large_text);
        }
        if (Game_Control.control.get_save_data().player_health == 0)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.red);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(new Rect(mid_w - 65, mid_h + 30, 130, 30), GUIContent.none);

            GUI.Label(new Rect(mid_w-65, mid_h+30, 130, 30), "YOU DIED", large_text);
        }
        if (jump_text_counter > 0)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.black);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(new Rect(mid_w - 165, mid_h + 100, 330, 30), GUIContent.none);

            GUI.Label(new Rect(mid_w - 165, mid_h + 100, 330, 30), "DOUBLE JUMP UNLOCKED", large_text);
        }

        if (victory_text_counter > 0)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.black);
            texture.Apply();
            GUI.skin.box.normal.background = texture;
            GUI.Box(new Rect(mid_w - 55, mid_h + 30, 110, 30), GUIContent.none);

            GUI.Label(new Rect(mid_w - 55, mid_h + 30, 120, 30), "VICTORY", large_text);
        }
    }

    public void show_save_text()
    {
        save_counter = save_counter_max;
    }

    public void show_doublejump_text()
    {
        jump_text_counter = jump_text_counter_max;
    }

    public void show_victory_text()
    {
        victory_text_counter = victory_text_counter_max;
    }
}
