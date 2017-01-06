using UnityEngine;
using System.Collections;

/* File: Moving_Platform.cs
 * Author: Bill Bateman
 * Date Created: 1/6/17
 * 
 * Description:
 * 
 * 1/6/17:
 *  -moves the platform between two locations
 */
public class Moving_Platform : MonoBehaviour {

    public float speed = 5.0f;
    public GameObject destination_object;
    public float pause_time = 3.0f; //in seconds

    private Vector3 destination_position;
    private Vector3 initial_position;

    private int movement_state;
    private float counter;

	// Use this for initialization
    void Start()
    {
        initial_position = transform.position;
        destination_position = destination_object.transform.position;
        movement_state = 0;
    }
	
	void FixedUpdate () {

        if (movement_state == 0)
        {
            //move to destination
            counter += Time.deltaTime;
            transform.position = Vector3.Lerp(initial_position, destination_position, speed * counter);

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
            transform.position = Vector3.Lerp(destination_position, initial_position, speed * counter);

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
}
