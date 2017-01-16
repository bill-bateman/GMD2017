using UnityEngine;
using System.Collections;

public class Checkpoint_Platform : MonoBehaviour {

    public int checkpoint_index;
    public Attractor starting_attractor;

    private int animation_count;
    private static int animation_max = 100;

	// Use this for initialization
	void Start () {
        animation_count = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (animation_count > 0)
        {
            animation_count--;
        }

        /*
        if (animation_count > 0)
        {
            //do a thing
            animation--;
            Light light = GetComponent<Light>();
            light.intensity = (((float)animation_max - animation_count)/animation_max) * (8);
        }
        else
        {
            //boring static thing
            if (Game_Control.control.get_current_checkpoint_index() == checkpoint_index)
            {
                //glow
                Light light = GetComponent<Light>();
                light.intensity = 8;
            }
            else
            {
                //no glow
                Light light = GetComponent<Light>();
                light.intensity = 0;
            }
        }
        */
	}

    public bool perform_save()
    {
        if (animation_count == 0)
        {
            //save and start animation
            animation_count = animation_max;
            Game_Control.control.set_current_checkpoint_index(checkpoint_index);
            Game_Control.control.Save();
            return true;
        }
        return false;
    }
}
