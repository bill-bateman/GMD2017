using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* File: Moving_Platform.cs
 * Author: Bill Bateman
 * Date Created: 1/6/17
 * 
 * Description:
 * 
 * 1/6/17:
 *  -moves the platform between two locations
 * 1/9/17:
 *  -moved sticky platform function into this file
 *  -also improved/fixed it
 */
public class Moving_Platform : MonoBehaviour {

    public float frame_count = 100.0f;
    public GameObject destination_object;
    public float pause_time = 3.0f; //in seconds

    private Vector3 destination_position;
    private Vector3 initial_position;

    private int movement_state;
    private float counter;
    private Vector3 offset_per = new Vector3();

    private List<GameObject> on_platform = new List<GameObject>();

	// Use this for initialization
    void Start()
    {
        initial_position = transform.position;
        destination_position = destination_object.transform.position;
        movement_state = 0;

        offset_per.x = (destination_position.x - initial_position.x) / frame_count;
        offset_per.y = (destination_position.y - initial_position.y) / frame_count;
        offset_per.z = (destination_position.z - initial_position.z) / frame_count;

    }
	
	void FixedUpdate () {

        Vector3 moved = new Vector3(0, 0, 0);

        if (movement_state == 0)
        {
            //move to destination
            counter += Time.deltaTime;
            //transform.position = Vector3.Lerp(initial_position, destination_position, speed * counter);
            transform.position += offset_per;

            foreach (GameObject g in on_platform) //move all objects on the platform
            {
                g.transform.position += offset_per;
            }

            float dist = Vector3.Magnitude(transform.position - destination_position);
            if (dist < 0.1f)
            {
                movement_state = 1;
                counter = 0;
            }
        }
        else if (movement_state == 1)
        {
            //pause at destination
            counter+=Time.deltaTime;
            if (counter > pause_time)
            {
                counter = 0;
                movement_state = 2;
            }
        }
        else if (movement_state == 2)
        {
            //move to initial
            counter += Time.deltaTime;
            //transform.position = Vector3.Lerp(destination_position, initial_position, speed * counter);
            transform.position -= offset_per;

            foreach (GameObject g in on_platform)
            {
                g.transform.position -= offset_per;
            }

            float dist = Vector3.Magnitude(transform.position - initial_position);
            if (dist < 0.1f)
            {
                movement_state = 3;
                counter = 0;
            }

        }
        else if (movement_state == 3)
        {
            //pause at source
            counter += Time.deltaTime;
            if (counter > pause_time)
            {
                movement_state = 0;
                counter = 0;
            }
        }

	}


    //objects on the platform must report themselves to these functions below
    //they will then be automatically moved by the FixedUpdate function above
    public void add_item_to_list(GameObject g)
    {
        if (!on_platform.Contains(g)) on_platform.Add(g);
    }
    public bool remove_item_from_list(GameObject g)
    {
        return on_platform.Remove(g);
    }
}
