using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/* File: Game_Control.cs
 * Author: Bill Bateman
 * Date Created: 1/6/17
 * 
 * Description:
 * 
 * 1/6/17:
 *  -stores useful information to be passed between scenes
 */
public class Game_Control : MonoBehaviour {

    public static Game_Control control; //acts as a singleton
    private SaveData data = new SaveData();
    private bool loadable = false;

    //for the options menu
    public bool is_options_menu_open = false;
    public bool is_pause_menu_open = false;
    public Vector3 curr_cam_position;
    public Quaternion curr_cam_rotation;

    //NOTE: debugging thing - use this to revert to default data
    private static bool delete_data_on_start_up = true;


    /*** GETTERS AND SETTERS ***/

    public void collect_cat_coin(int index)
    {
        if (index > SaveData.total_cats || index < 0) return;

        data.cats_collected++;
        data.is_cat_collected[index] = true;

        //increase health by one
        data.player_health++;
        if (data.player_health > data.player_max) data.player_health = data.player_max;

        //auto-save on collect
        Save();
    }

    public bool check_cat_coin(int index) {
        if (index > SaveData.total_cats || index < 0) return true; //true => already collected
        return data.is_cat_collected[index]; 
    }
    public bool does_load_file_exist() { return loadable; }

    public void take_damage(int d)
    {
        data.player_health -= d;
    }
    public void full_health()
    {
        data.player_health = data.player_max;
    }

    public int get_volume() { return data.volume; }
    public void set_volume(int v) { data.volume = v; }
    public bool get_x_axis_inverted() { return data.invert_cam_x_axis; }
    public void set_x_axis_inverted(bool v) { data.invert_cam_x_axis = v; }
    public bool get_y_axis_inverted() { return data.invert_cam_y_axis; }
    public void set_y_axis_inverted(bool v) { data.invert_cam_y_axis = v; }

    public void set_current_checkpoint_index(int i) { data.current_checkpoint = i; }
    public int get_current_checkpoint_index() { return data.current_checkpoint; }

    public SaveData get_save_data() { return data; }


    /*** SAVING AND LOADING THE DATA ***/
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.OpenOrCreate); //uses the persistent data path for the application (for windows sthg like appData/roaming/...)
        
        bf.Serialize(fs, data);
        fs.Close();
    }
    public void Load()
    {
        //check file exists
        if (File.Exists(Application.persistentDataPath + "/saveData.dat"))
        {
            //load
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open);

            try
            {
                data = (SaveData)bf.Deserialize(fs);
                loadable = true;
            }
            catch (Exception e)
            {
                //handle corrupted data
                data.defaultData(true); //revert to default data
                loadable = false;
                print(e.StackTrace);
            }
            fs.Close();
        }
        else
        {
            //create default data
            data.defaultData(true);
        }

        
    }

    public void NewGame()
    {
        //create default data, overwrite save file
        data.defaultData(false);
        Save();

        //go to the first scene
        SceneManager.LoadScene(data.current_scene, LoadSceneMode.Single);
    }
    public void LoadGame()
    {
        //go to the saved scene
        SceneManager.LoadScene(data.current_scene, LoadSceneMode.Single);
    }

	// Use this for initialization the singleton
	void Awake () {
	    //Awake happens before Start
        if (control == null)
        {
            //create object (none exists)
            DontDestroyOnLoad(gameObject);
            if (delete_data_on_start_up)
                data.defaultData(true);
            else
                Load(); //load data from file
            control = this;
        }
        else
        {
            //control object already exists
            Destroy(gameObject);
        }
	}
	
}

[Serializable]
public class SaveData
{
    //all the data to be saved to file
    public int player_health;
    public int player_max;

    public int cats_collected;
    public static int total_cats = 5; //constant

    public bool[] is_cat_collected = new bool[total_cats];

    public string current_scene;
    public int current_checkpoint;

    public bool can_double_jump; //synonymous with having killed the doggo

    //VARIABLES FOR OPTIONS MENU
    public int volume;
    public bool invert_cam_x_axis;
    public bool invert_cam_y_axis;

    public void defaultData(bool reset_options)
    {
        player_health = 5;
        player_max = 5;
        cats_collected = 3;
        for (int i = 0; i < total_cats; i++) { is_cat_collected[i] = false; }

        current_scene = "world_1";
        current_checkpoint = 0;

        can_double_jump = false;

        if (reset_options)
        {
            volume = 0;
            invert_cam_x_axis = false;
            invert_cam_y_axis = true;
        }
        
    }
}