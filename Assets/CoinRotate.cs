using UnityEngine;
using System.Collections;

/* File: CoinRotate.cs
 * Author: Bill Bateman
 * Date Created: 1/6/17
 * 
 * Description:
 * 
 * 1/6/17:
 *  -rotates the collectibles
 *  -added trigger enter, to remove object when player touches it
 */
public class CoinRotate : MonoBehaviour {

    private float angle;
    private float angle_incr;

	// Use this for initialization
	void Start () {
        angle_incr = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
	   //rotate around the up axis
       gameObject.transform.Rotate(gameObject.transform.forward, angle_incr, Space.World);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            gameObject.SetActive(false);
        }
    }
}
