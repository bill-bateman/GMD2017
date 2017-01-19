using UnityEngine;
using UnityEngine.SceneManagement;
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

    public UI_Handler ui_handler;
    public GameObject robot_renderer;

    public float speed;
    public float rot_speed;
    public float jump_force = 500.0f;

    public AudioClip impact, jump, walk, damage, death, save;


    private AudioSource audio_s;
    private Attractor curr_attr;
    private Rigidbody rb;
    private bool is_on_ground;
    private bool is_changing_attractors;

    private int jump_counter;
    private static int jump_counter_max = 10;

    private Vector3 last_safe_spot;
    private int damaged_counter;
    private static int damanged_counter_max = 100;

    private int damaged_moving_counter;
    private static int damaged_moving_counter_max = 25;
    private Vector3 damaged_old_pos;

    private bool double_jump_tracker;

    /**************************************************************************************
     *                                   UNITY FUNCTIOBNS                                 *
     **************************************************************************************/
    
	// Use this for initialization
	void Start () {
        audio_s = GetComponent<AudioSource>();

        is_on_ground = true;
        is_changing_attractors = false;
        double_jump_tracker = false;

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

        last_safe_spot = transform.position;
        damaged_counter = 0;
        damaged_moving_counter = 0;
	}

    void update_safe_spot(Collider other)
    {
        if (other.tag == "planet" || other.tag == "jumpable" || other.tag == "checkpoint_jumpable")
        {
            last_safe_spot = transform.position;
        }
    }
    void check_for_damage()
    {
        //deal damage
        if (damaged_counter == 0)
        {
            Game_Control.control.take_damage(1);
            damaged_counter = damanged_counter_max;

            if (Game_Control.control.get_save_data().player_health == 0)
            {
                audio_s.clip = death;
                audio_s.Play();

                Time.timeScale = 0; //stop time
            }
            else
            {
                audio_s.clip = damage;
                audio_s.Play();
            }
        }
    }

    public bool did_just_take_damage()
    {
        return (damaged_counter != 0);
    }

    public bool is_on_planet3()
    {
        return (curr_attr.name == "Planet3_Body" && !is_changing_attractors);
    }

    //called on entering collision (one of the 2 objects in the collision needs to have isTrigger checked for this to occur)
    void OnTriggerEnter(Collider other) {
        update_safe_spot(other);


        if (other.tag == "planet") { 
            is_on_ground = true;
            double_jump_tracker = false;
            if ((Attractor)other.gameObject.GetComponent<Attractor>() == curr_attr) { 
                is_changing_attractors = false; 
            }

            audio_s.clip = impact;
            audio_s.Play();
        }
        else if (other.tag == "jumpable")
        {
            is_on_ground = true;
            double_jump_tracker = false;
            is_changing_attractors = false;

            audio_s.clip = impact;
            audio_s.Play();
        }
        else if (other.tag == "sticky_jumpable")
        {
            is_on_ground = true;
            double_jump_tracker = false;
            is_changing_attractors = false; 
            (other.gameObject.GetComponent<Moving_Platform>()).add_item_to_list(gameObject);

            audio_s.clip = impact;
            audio_s.Play();
        }
        else if (other.tag == "checkpoint_jumpable")
        {
            double_jump_tracker = false;
            is_on_ground = true;
            is_changing_attractors = false; 
            if ((other.gameObject.GetComponent<Checkpoint_Platform>()).perform_save())
            {
                ui_handler.show_save_text();
            }
            Game_Control.control.full_health(); //back to full health

            audio_s.clip = save;
            audio_s.Play();
        }
        else if (other.tag == "hazard")
        {
            check_for_damage();
            damaged_old_pos = transform.position;
            damaged_moving_counter = damaged_moving_counter_max;

        }
        else if (other.tag == "boss_hit")
        {
            MessageArgs args = new MessageArgs();
            other.transform.parent.SendMessage("is_damaged", args);

            if (!args.result)
                check_for_damage();
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
        update_safe_spot(other);

        if (other.tag == "planet" || other.tag == "jumpable" || other.tag == "checkpoint_jumpable")
        {
            is_on_ground = true;
            double_jump_tracker = false;
        }
        else if (other.tag == "sticky_jumpable")
        {
            if (!is_on_ground)
            {
                is_on_ground = true;
                double_jump_tracker = false;
                (other.gameObject.GetComponent<Moving_Platform>()).add_item_to_list(gameObject);
            }
        }
        else if (other.tag == "hazard")
        {
            check_for_damage();
        }
        else if (other.tag == "boss_hit")
        {
            MessageArgs args = new MessageArgs();
            other.transform.parent.SendMessage("is_damaged", args);

            if (!args.result)
                check_for_damage();
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

                if (is_on_ground && Vector3.Magnitude(movement) > 0.001f)
                {
                    audio_s.clip = walk;
                    if (!audio_s.isPlaying)
                        audio_s.Play();
                }
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
            if (input_module.get_jump() && (is_on_ground || Game_Control.control.get_save_data().can_double_jump && !double_jump_tracker) && jump_counter == 0)
            {
                if (!is_on_ground)
                    double_jump_tracker = true;

                Vector3 up = Vector3.Normalize(gameObject.transform.position - curr_attr.gameObject.transform.position);
                rb.AddForce(jump_force * up);

                jump_counter = jump_counter_max; //jump cooldown

                audio_s.clip = jump;
                audio_s.Play();
            }

        }
        if (jump_counter > 0) { jump_counter--; }

        /*** GRAVITY ***/
        curr_attr.attract_player(rb, is_changing_attractors);
    }

    void Update()
    {
        if (damaged_counter > 0)
        {
            damaged_counter--;
            //gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
            robot_renderer.GetComponent<Renderer>().material.color = Color.red;

            if (damaged_moving_counter > 0)
            {
                transform.position = Vector3.Slerp(last_safe_spot, damaged_old_pos, (float)damaged_moving_counter / damaged_moving_counter_max);
                damaged_moving_counter--;
            }
        }
        if (damaged_counter < 5)
        {
            //gameObject.GetComponentInChildren<Renderer>().material.color = Color.white;
            robot_renderer.GetComponent<Renderer>().material.color = Color.white;
        }
        if (damaged_counter == 0)
        {
            damaged_moving_counter = 0;
        }

        if (Game_Control.control.get_save_data().player_health == 0 && damaged_counter == 0)
        {
            Game_Control.control.full_health();
            Time.timeScale = 1;
            //restart this scene
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
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


public class MessageArgs
{
    public MessageArgs()
    {
    }

    public bool result = false;
}