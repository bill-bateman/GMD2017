using UnityEngine;
using System.Collections;

/* File: Sticky_Platform.cs
 * Author: Bill Bateman
 * Date Created: 1/6/17
 * 
 * Description:
 * 
 * 1/6/17:
 *  -keeps the player stuck to a moving platform
 */
public class Sticky_Platform : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            other.gameObject.transform.parent.parent = gameObject.transform.parent;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "player")
        {
            other.gameObject.transform.parent.parent = null;
        }
    }
}
