using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options_Values : MonoBehaviour {

    public Slider volume_slider;
    public Toggle x_axis;
    public Toggle y_axis;

	// Use this for initialization
	void Start () {
	    int temp_volume = Game_Control.control.get_volume();
        bool temp_x_invert = Game_Control.control.get_x_axis_inverted();
        bool temp_y_invert = Game_Control.control.get_y_axis_inverted();

        volume_slider.value = temp_volume;
        x_axis.isOn = temp_x_invert;
        y_axis.isOn = temp_y_invert;
	}

    public void update_save_data()
    {
        Game_Control.control.set_volume((int)volume_slider.value);
        Game_Control.control.set_x_axis_inverted(x_axis.isOn);
        Game_Control.control.set_y_axis_inverted(y_axis.isOn);

        Game_Control.control.Save();
    }
}
