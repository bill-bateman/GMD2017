using UnityEngine;
using System.Collections;

/* File: Platform_Rotation.cs
 * Author: Bill Bateman
 * Date Created: 1/6/17
 * 
 * Description:
 * 
 * 1/6/17:
 *  -used to rotate the platform so that 'down' is towards the planet ('up' is away from the planet)
 */
public class Platform_Rotation : MonoBehaviour {

    public GameObject planet; //based on the planet's center, the platform's 'up' vector will point away from the planet
    public float rot_speed = 5.0f;
	
	// Update is called once per frame
	void Update () {
        Vector3 p_center = planet.transform.position;
        Vector3 new_up = Vector3.Normalize(transform.position - p_center);

        Quaternion target_rot = Quaternion.FromToRotation(transform.up, new_up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, target_rot, rot_speed * Time.deltaTime);
	}
}
