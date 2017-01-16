using UnityEngine;
using System.Collections;

public class Pause_Camera : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        transform.position = Game_Control.control.curr_cam_position;
        transform.rotation = Game_Control.control.curr_cam_rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
