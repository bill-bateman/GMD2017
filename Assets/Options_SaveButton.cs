using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options_SaveButton : MonoBehaviour {

    public Options_Values option_values;

    // Use this for initialization
    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        //close the options scene
        option_values.update_save_data();
    }
}
