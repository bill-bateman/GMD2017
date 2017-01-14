using UnityEngine;
using System.Collections;

public class CatBarrier : MonoBehaviour {

    public int num_cats_needed;
    	
	// Update is called once per frame
	void Update () {
        if (Game_Control.control.get_save_data().cats_collected >= num_cats_needed)
        {
            gameObject.SetActive(false);
        }
	}
}
