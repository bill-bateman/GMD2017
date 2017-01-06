using UnityEngine;
using System.Collections;

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
