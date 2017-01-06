using UnityEngine;
using System.Collections;

/* File: Player_Movement.cs
 * Author: Bill Bateman
 * Date Created: 1/5/17
 * 
 * Description:
 * 
 * 1/5/17:
 *  -used for performing movement (character movement, camera movement)
 *  -also handles the attractor (i.e. gravity to different planets)
 */

public class Player_Movement : MonoBehaviour {

    /**************************************************************************************
     *                                     CLASS VARIABLES                                *
     **************************************************************************************/

    public Input_Module input_module;
    public Player_Camera cam;
    public Attractor default_attr;

    public float speed;
    public float rot_speed;
    public float jump_force = 500.0f;


    private Attractor curr_attr;
    private Rigidbody rb;
    private bool is_on_ground;
    private bool is_changing_attractors;


    /**************************************************************************************
     *                                   UNITY FUNCTIOBNS                                 *
     **************************************************************************************/

	// Use this for initialization
	void Start () {
        is_on_ground = true;
        is_changing_attractors = false;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;

        curr_attr = default_attr;
        cam.update_offset(curr_attr.camera_offset);
	}

    //called on entering collision (one of the 2 objects needs to have isTrigger checked)
    void OnTriggerEnter(Collider other) {
        if (other.tag == "planet") { 
            is_on_ground = true;
            if ((Attractor)other.gameObject.GetComponent<Attractor>() == curr_attr) { 
                is_changing_attractors = false; 
            }
        }
        else if (other.tag == "jumpable")
        {
            is_on_ground = true;
        }
    }

    //called on leaving collision (one of the 2 objects needs to have isTrigger checked)
    void OnTriggerExit(Collider other) {
        if (other.tag == "planet") { 
            is_on_ground = false;  
        }
    }


    //called at constant intervals
    void FixedUpdate()
    {
        if (!is_changing_attractors)
        {
            //don't allow player movement if changing planets (can go into orbit - i.e. infinite free fall)

            /*** MOTION - STRAFE ***/
            if (input_module.is_moving_horizontal())
            {
                float strafe = input_module.get_strafe_motion();

                Vector3 trans = new Vector3(strafe, 0.0f, 0.0f);
                Vector3 direction = gameObject.transform.localRotation * trans; //the direction to move in terms of the object's current rotation
                direction.Normalize(); //normalize to x-z plane

                Vector3 add = direction * (speed * Time.deltaTime); //control movement speed
                gameObject.transform.localPosition += add; //changing the actual position
            }
            if (input_module.is_moving_vertical())
            {
                Vector3 trans = new Vector3(0.0f, 0.0f, input_module.get_fwd_motion());
                Vector3 direction = gameObject.transform.localRotation * trans;
                direction.Normalize();
                Vector3 add = direction * (speed * Time.deltaTime);
                gameObject.transform.localPosition += add;
            }


            /*** MOTION - ROTATION ***/
            if (input_module.is_camera_moving_horizontal())
            {
                float rot = input_module.get_camera_horizontal();
                gameObject.transform.Rotate(Vector3.up, rot);
            }
            if (input_module.is_camera_moving_vertical())
            {
                //update the cameras position (rotate up/down)
                cam.rotate_vertically(input_module.get_camera_vertical() > 0.0f);
            }


            /*** MOTION - JUMPING ***/
            if (input_module.get_jump() && is_on_ground)
            {
                Vector3 up = Vector3.Normalize(gameObject.transform.position - curr_attr.gameObject.transform.position);
                rb.AddForce(jump_force * up);
            }

        }

        /*** GRAVITY ***/
        curr_attr.attract_object(rb, is_changing_attractors);
    }



    /**************************************************************************************
     *                                   PUBLIC FUNCTIOBNS                                *
     **************************************************************************************/

    public void update_gravity_attractor(Attractor g, float springboard_power)
    {
        curr_attr = g;
        cam.update_offset(curr_attr.camera_offset);
        is_changing_attractors = true;

        //springboard towards the new attractor
        Vector3 up = Vector3.Normalize(gameObject.transform.position - curr_attr.gameObject.transform.position);
        rb.AddForce(-springboard_power * up);
    }

    public bool get_changing_attractor() { return is_changing_attractors; }
}
