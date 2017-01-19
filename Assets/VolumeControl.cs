using UnityEngine;
using System.Collections;

public class VolumeControl : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        //update volume
        AudioListener.volume = ((float)Game_Control.control.get_save_data().volume) / 10.0f;
	}
}
