using UnityEngine;
using System.Collections;

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
}
