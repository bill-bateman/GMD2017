using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Continue_Button : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        //close the options scene
        Game_Control.control.is_pause_menu_open = false;
    }
}
