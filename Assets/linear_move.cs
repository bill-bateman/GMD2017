using UnityEngine;
using System.Collections;

public class linear_move : MonoBehaviour {

    public Vector3 movement_per_frame = new Vector3(0.1f, 0.02f, 0.0f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += movement_per_frame;
	}
}
