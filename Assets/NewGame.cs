using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
	}

    void TaskOnClick()
    {
        //new game button has been clicked
        Game_Control.control.NewGame();
    }
	
}
