using UnityEngine;
using System.Collections;

/* File: Player_Camera.cs
 * Author: Bill Bateman
 * Date Created: 1/5/17
 * 
 * Description:
 * 
 * 1/5/17:
 *  -follows the target object and looks at it
 */

public class Player_Camera : MonoBehaviour {

    public GameObject target;
    public float angle_incr = 0.5f;

    public float min_boundary = -5.0f;
    public float max_boundary = -20.0f;

    public float cam_speed = 25.0f;

    private Vector3 offset;

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

	// Update is called once per frame
	void LateUpdate () {

        //update the position of the camera, and look at the target object
        Quaternion rotation = Quaternion.Euler(target.transform.eulerAngles.x, target.transform.eulerAngles.y, target.transform.eulerAngles.z);
        transform.position = Vector3.Slerp(transform.position, target.transform.position - (rotation * offset), Mathf.Abs(Vector3.Magnitude(target.transform.position - (rotation * offset)) / 10.0f));

        transform.LookAt(target.transform, target.transform.up);

        Game_Control.control.curr_cam_position = transform.position;
        Game_Control.control.curr_cam_rotation = transform.rotation;

        //update volume
        AudioListener.volume = ((float)Game_Control.control.get_save_data().volume) / 10.0f;
	}


    public void update_offset(Vector3 v) { offset = v; }
    public Vector3 current_offset() { return offset; }

    public void rotate_vertically(float rot_val)
    {
        //called on input
        //rotate the camera (i.e. look up or down)
        float direction = rot_val;
        float new_y = offset.y + (direction * angle_incr);

        if (new_y > min_boundary) new_y = min_boundary;
        if (new_y < max_boundary) new_y = max_boundary;

        Vector3 new_offset = new Vector3(offset.x, new_y, offset.z);
        offset = new_offset;
    }
}
