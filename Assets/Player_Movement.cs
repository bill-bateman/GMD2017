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
 * 1/9/17:
 *  -changed how movement is modeled (i.e. want from more tank-like controls to more action-y controls)
 *      -essentially, the camera is no longer always behind the player (i.e. doesn't depend on the player's facing)
 *  -also changed how collision with ground is handled (uses small box collider on the bottom) as well as how moving platforms are handled (see Moving_Platform.cs)
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

    private int jump_counter;
    private static int jump_counter_max = 10;


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

        jump_counter = 0;

        Player_Spawner ps = gameObject.GetComponent<Player_Spawner>();
        Vector3 tmp = ps.get_location_for_index(Game_Control.control.get_current_checkpoint_index());
        gameObject.transform.position = tmp;

        Attractor a = ps.get_attractor_for_index(Game_Control.control.get_current_checkpoint_index());
        if (a != null)
        {
            curr_attr = a;
            cam.update_offset(a.camera_offset);
        }
	}

    //called on entering collision (one of the 2 objects in the collision needs to have isTrigger checked for this to occur)
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
        else if (other.tag == "sticky_jumpable")
        {
            is_on_ground = true;
            (other.gameObject.GetComponent<Moving_Platform>()).add_item_to_list(gameObject);
        }
        else if (other.tag == "checkpoint_jumpable")
        {
            is_on_ground = true;
            (other.gameObject.GetComponent<Checkpoint_Platform>()).perform_save();
        }
    }

    //called on leaving collision (one of the 2 objects needs to have isTrigger checked)
    void OnTriggerExit(Collider other) {
        if (other.tag == "planet" || other.tag == "jumpable" || other.tag == "checkpoint_jumpable") { 
            is_on_ground = false;  
        }
        else if (other.tag == "sticky_jumpable")
        {
            is_on_ground = false;
            (other.gameObject.GetComponent<Moving_Platform>()).remove_item_from_list(gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "planet" || other.tag == "jumpable" || other.tag == "checkpoint_jumpable")
        {
            is_on_ground = true;
        }
        else if (other.tag == "sticky_jumpable")
        {
            if (!is_on_ground)
            {
                is_on_ground = true;
                (other.gameObject.GetComponent<Moving_Platform>()).add_item_to_list(gameObject);
            }
        }
    }


    //called at constant intervals
    void FixedUpdate()
    {
        transform.localScale = new Vector3(1, 1, 1);

        if (!is_changing_attractors)
        {
            //don't allow player movement if changing planets (can go into orbit - i.e. infinite free fall)

            //save overall movement of player
            Vector3 movement = new Vector3(0, 0, 0);

            /*** MOTION - PLAYER MOVEMENT ***/
            if (input_module.is_moving_horizontal())
            {
                float strafe = input_module.get_strafe_motion();

                Vector3 trans = new Vector3(strafe, 0.0f, 0.0f);
                Vector3 direction = cam.transform.rotation * trans;
                //Vector3 direction = Vector3.Cross(trans, get_updated_up_vector());

                Vector3 add = direction * (speed * Time.deltaTime); //control movement speed
                movement += add;
            }
            if (input_module.is_moving_vertical())
            {
                Vector3 trans = new Vector3(0, 0, input_module.get_fwd_motion());
                Quaternion r = cam.transform.rotation;
                
                Vector3 direction = r * trans;
                float mag = Vector3.Magnitude(direction);
                direction = Vector3.Normalize(Vector3.ProjectOnPlane(direction, transform.up)) * mag;

                Vector3 add = direction * (speed * Time.deltaTime);
                movement += add;
            }

            if (input_module.is_moving_horizontal() || input_module.is_moving_vertical())
            {
                transform.localPosition += movement;

                //change the direction of the player to face where we are now moving
                Quaternion euler_rot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                Vector3 cam_const = euler_rot * cam.current_offset();

                Vector3 next_pos = transform.position + movement;
                transform.LookAt(next_pos, get_updated_up_vector());

                Quaternion euler_rot2 = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                Vector3 new_offset = Quaternion.Inverse(euler_rot2) * cam_const;
                cam.update_offset(new_offset);
            }


            /*** MOTION - CAMERA MOVEMENT ***/
            if (input_module.is_camera_moving_horizontal())
            {
                //rotate the camera, but leave the character alone
                //do this by changing the camera's offset

                //x and z coordinates of offset are like the two coordinates of a point moving along a circle
                //i.e. there is a constant radius r, and changing angle theta
                float x = cam.current_offset().x; float z = cam.current_offset().z;
                float r = Mathf.Sqrt(x * x + z * z);
                float theta = Mathf.Atan2(z, x);

                float rot = input_module.get_camera_horizontal();
                theta += rot * rot_speed;

                x = r * Mathf.Cos(theta);
                z = r * Mathf.Sin(theta);

                cam.update_offset(new Vector3(x, cam.current_offset().y, z));
            }
            if (input_module.is_camera_moving_vertical() && Mathf.Abs(input_module.get_camera_vertical()) > 0.1f)
            {
                //update the cameras position (rotate up/down)
                cam.rotate_vertically(input_module.get_camera_vertical());
            }


            /*** MOTION - JUMPING ***/
            if (input_module.get_jump() && is_on_ground && jump_counter == 0)
            {
                Vector3 up = Vector3.Normalize(gameObject.transform.position - curr_attr.gameObject.transform.position);
                rb.AddForce(jump_force * up);

                jump_counter = jump_counter_max; //jump cooldown
            }

        }
        if (jump_counter > 0) { jump_counter--; }

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

    public Vector3 get_updated_up_vector()
    {
        Vector3 up = Vector3.Normalize(transform.position - curr_attr.transform.position);
        return up;
    }
}
