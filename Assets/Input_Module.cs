using UnityEngine;
using System.Collections;

/* File: Input_Module.cs
 * Author: Bill Bateman
 * Date Created: 1/5/17
 * 
 * Description:
 * 
 * 1/5/17:
 *  -used to interact with the inputs for the game (keyboard, xbox controller)
 * 1/9/17:
 *  -changed to use the invert camera axes options from Game_Control
 *  -changed how input axes are handled to allow analog values from xbox controller
 */

public class Input_Module : MonoBehaviour
{

    public float camera_horizontal_rotation_speed = 2.5f;
    public float camera_vertical_rotation = 5.0f;
    public float controller_rotation_multiplier = 5.0f;

    private bool is_controller; //as opposed to keyboard

    //movement vars
    private float fwd_back, left_right;
    private float cam_movement_updown, cam_movement_leftright;
    private bool jump_button, camera_h, camera_v, movement_h, movement_v;

    //CONTROLLER BINDINGS
    private KeyCode controller_jump_button = KeyCode.Joystick1Button0; //A
    private string controller_movement_x_axis = "Horizontal"; //left stick
    private string controller_movement_y_axis = "Vertical";
    private string controller_cam_x_axis = "Cam_Horizontal"; //right stick
    private string controller_cam_y_axis = "Cam_Vertical";


    //KEYBOARD BINDINGS
    private KeyCode keyboard_jump_button = KeyCode.Space;

    private KeyCode keyboard_movement_up = KeyCode.W;
    private KeyCode keyboard_movement_down = KeyCode.S;
    private KeyCode keyboard_movement_left = KeyCode.A;
    private KeyCode keyboard_movement_right = KeyCode.D;

    private KeyCode keyboard_cam_up = KeyCode.UpArrow;
    private KeyCode keyboard_cam_down = KeyCode.DownArrow;
    private KeyCode keyboard_cam_left = KeyCode.LeftArrow;
    private KeyCode keyboard_cam_right = KeyCode.RightArrow;

    // Use this for initialization
    void Start()
    {
        is_controller = false;
        jump_button = camera_h = camera_v = movement_h = movement_v = false;
        fwd_back = left_right = cam_movement_updown = cam_movement_leftright = 0.0f;
    }

    public bool get_jump() {
        if (jump_button == true) {
            jump_button = false;
            return true;
        }
        return jump_button;
    }
    public bool is_camera_moving_horizontal() { return camera_h; }
    public bool is_camera_moving_vertical() { return camera_v; }
    public bool is_moving_horizontal() { return movement_h; }
    public bool is_moving_vertical() { return movement_v; }

    public float get_fwd_motion() { return fwd_back; }
    public float get_strafe_motion() { return left_right; }
    public float get_camera_vertical() { return Game_Control.control.get_y_axis_inverted() ? -cam_movement_updown : cam_movement_updown; }
    public float get_camera_horizontal() { return Game_Control.control.get_x_axis_inverted() ? -cam_movement_leftright : cam_movement_leftright; }










    // Update is called once per frame
    void Update()
    {
        is_controller = false;
        string[] names = Input.GetJoystickNames();
        for (int x = 0; x < names.Length; x++)
        {
            if (names[x].Length == 33) { is_controller = true; } //XBOX controller detected
        }

        if (is_controller)
        {

            //motion
            fwd_back = Input.GetAxis(controller_movement_y_axis);
            left_right = Input.GetAxis(controller_movement_x_axis);

            //camera rotation
            cam_movement_updown = Input.GetAxis(controller_cam_y_axis) ;
            cam_movement_leftright = Input.GetAxis(controller_cam_x_axis) * controller_rotation_multiplier;

            camera_h = camera_v = movement_h = movement_v = true;


            //jump
            if (!jump_button)
                jump_button = Input.GetKeyDown(controller_jump_button);

        }
        else
        {
            //get inputs from keyboard

            //player motion
            bool move_left = Input.GetKey(keyboard_movement_left);
            bool move_right = Input.GetKey(keyboard_movement_right);
            bool move_fwd = Input.GetKey(keyboard_movement_up);
            bool move_back = Input.GetKey(keyboard_movement_down);

            if (move_left ^ move_right)
            {
                left_right = (move_left ? -1.0f : 1.0f); movement_h = true;
            }
            else { left_right = 0.0f; movement_h = false; }

            if (move_fwd ^ move_back)
            {
                fwd_back = (move_back ? -1.0f : 1.0f); movement_v = true;
            }
            else { fwd_back = 0.0f; movement_v = false; }


            //camera rotation
            bool rot_left = Input.GetKey(keyboard_cam_left);
            bool rot_right = Input.GetKey(keyboard_cam_right);
            if (rot_left ^ rot_right)
            {
                cam_movement_leftright = camera_horizontal_rotation_speed * (rot_left ? -1.0f : 1.0f);
                camera_h = true;
            }
            else { cam_movement_leftright = 0.0f; camera_h = false; }

            //vertical rotation (camera)
            bool rot_up = Input.GetKey(keyboard_cam_up);
            bool rot_down = Input.GetKey(keyboard_cam_down);
            if (rot_up ^ rot_down)
            {
                cam_movement_updown = (rot_down ? -1.0f : 1.0f); camera_v = true;
            }
            else { cam_movement_updown = 0.0f; camera_v = false; }


            //JUMP
            if (!jump_button)
                jump_button = (Input.GetKeyDown(keyboard_jump_button));


        }
    }
}
