using UnityEngine;
using System.Collections;

/* File: Attractor.cs
 * Author: Bill Bateman
 * Date Created: 1/5/17
 * 
 * Description:
 * 
 * 1/5/17:
 *  -used to apply gravity to objects (e.g. player, enemies)
 *  -also correctly rotates these objects to stay on the planet's surface
 */

public class Attractor : MonoBehaviour {

    public float local_grav;
    public Vector3 camera_offset;


    //rotation speed used when angling a rigidbody to have it's 'down' point towards the planet center
    private float rb_rot_speed = 10.0f;

    private float time_counter = 0;


    public void attract_player(Rigidbody obj, bool is_changing_attractor)
    {
        Vector3 center = gameObject.transform.position; //gets the planet's center
        float rot_speed = rb_rot_speed;


        if (is_changing_attractor) //i.e. we are using a springboard to travel to this planet
        {
            //sanity check - make sure we aren't travelling away from the planet's center (i.e. prevent infinite free fall)
            float curr_dist = Vector3.Distance(center, obj.position);
            float next_dist = Vector3.Distance(center, obj.position + Vector3.Normalize(obj.velocity));

            if (next_dist > curr_dist) obj.velocity = new Vector3(0, 0, 0);

            //slower camera rotation when changing planets
            time_counter += Time.deltaTime;
            if (time_counter < 0.3f) rot_speed = 0.05f;
            if (time_counter < 0.5f) rot_speed = 0.1f;
            else rot_speed = Mathf.Lerp(0.1f, 5.0f, time_counter / 5.0f);
        }
        else time_counter = 0;

        //ROTATION - keep the object correctly rotation (i.e. have down point towards the planet center, and have up be the same as the normal to the planet)
        Vector3 up = Vector3.Normalize(obj.transform.position - center);
        Quaternion target_rot = Quaternion.FromToRotation(obj.transform.up, up) * obj.transform.rotation;
        obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, target_rot, rot_speed * Time.deltaTime);

        //GRAVITY
        obj.AddForce(local_grav * up);
    }

    public void attract_object(Rigidbody obj)
    {
        Vector3 center = gameObject.transform.position; //gets the planet's center

        //ROTATION - keep the object correctly rotation (i.e. have down point towards the planet center, and have up be the same as the normal to the planet)
        Vector3 up = Vector3.Normalize(obj.transform.position - center);
        Quaternion target_rot = Quaternion.FromToRotation(obj.transform.up, up) * obj.transform.rotation;
        obj.transform.rotation = target_rot;

        //GRAVITY
        obj.AddForce(local_grav * up);
    }
}
