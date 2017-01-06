using UnityEngine;
using System.Collections;

/* File: Springboard_Platform.cs
 * Author: Bill Bateman
 * Date Created: 1/5/17
 * 
 * Description:
 * 
 * 1/5/17:
 *  -used to send the player to a new planet when a platform is triggered
 */

public class Springboard_Platform : MonoBehaviour {

    public Attractor new_attractor;
    public float springboard_power = 500.0f;
    public float springboard_dist = 5.0f;

    public float up_animation_speed = 5.0f;
    public float down_animation_speed = 1.0f;



    private int animation_state = 0;
    private Vector3 initial_pos;
    private Vector3 final_pos;

    void Start()
    {
        initial_pos = gameObject.transform.position;
        final_pos = initial_pos + (Vector3.Normalize(gameObject.transform.up) * springboard_dist);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            Player_Movement player = other.GetComponentInParent<Player_Movement>();
            player.update_gravity_attractor(new_attractor, springboard_power);

            animation_state = 1;
        }
    }

    void Update()
    {
        if (animation_state == 1)
        {
            //quickly go upwards
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, final_pos, up_animation_speed * Time.deltaTime);
            print(gameObject.transform.position + " - " + final_pos);
            if (Vector3.Magnitude(gameObject.transform.position - final_pos) < 0.1f)
                animation_state = 2;
        }
        else if (animation_state == 2)
        {
            //slowly go downwards
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, initial_pos, down_animation_speed * Time.deltaTime);
            if (Vector3.Magnitude(gameObject.transform.position - initial_pos) < 0.1f)
                animation_state=0;
        }

    }
}
