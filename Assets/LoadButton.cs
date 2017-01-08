using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button btn = gameObject.GetComponent<Button>();

        if (Game_Control.control.does_load_file_exist())
        {
            //enable the button
            btn.interactable = true;
            btn.onClick.AddListener(TaskOnClick);
        }
        else
        {
            //disable
            btn.interactable = false;
        }
	}

    void TaskOnClick()
    {
        //load button has been clicked
        Game_Control.control.LoadGame();
    }
}
