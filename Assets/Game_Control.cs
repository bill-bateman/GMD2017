using UnityEngine;
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

    //NOTE: debugging thing - use this to revert to default data
    private static bool delete_data_on_start_up = true;


    /*** GETTERS AND SETTERS ***/

    public void collect_cat_coin(int index)
    {
        if (index > SaveData.total_cats || index < 0) return;

        data.cats_collected++;
        data.is_cat_collected[index] = true;

        //auto-save on collect
        Save();
    }

    public bool check_cat_coin(int index) {
        if (index > SaveData.total_cats || index < 0) return true; //true => already collected
        return data.is_cat_collected[index]; 
    }


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
            }
            catch (Exception e)
            {
                //handle corrupted data
                data.defaultData(); //revert to default data
            }
            fs.Close();
        }
        else
        {
            //create default data
            data.defaultData();
            //create a save file
            Save();
        }

        
    }

	// Use this for initialization the singleton
	void Awake () {
	    //Awake happens before Start
        if (control == null)
        {
            //create object (none exists)
            DontDestroyOnLoad(gameObject);
            if (delete_data_on_start_up)
                data.defaultData();
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

    //temporary GUI labels to show data
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 30), "HEALTH: " + data.player_health + " / " + data.player_max);
        GUI.Label(new Rect(10, 30, 100, 30), "CATS:   " + data.cats_collected + " / " + data.cats_available);
    }
	
}

[Serializable]
class SaveData
{
    //all the data to be saved to file
    public int player_health;
    public int player_max;

    public int cats_collected;
    public int cats_available;
    public static int total_cats = 3; //constant

    public bool[] is_cat_collected = new bool[total_cats];

    public void defaultData()
    {
        player_health = 5;
        player_max = 5;
        cats_collected = 0;
        cats_available = 3;
        for (int i = 0; i < total_cats; i++) { is_cat_collected[i] = false; }
    }
}