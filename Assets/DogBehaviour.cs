using UnityEngine;
using System.Collections;

public class DogBehaviour : MonoBehaviour {

    public UI_Handler ui_handler;
    public GameObject rump;
    public GameObject body;
    public Attractor planet;

    private Rigidbody rb;

    private bool attack_mode;
    private int curr_health;
    private static int max_health = 3;

    private int dying_counter;
    private static int dying_counter_max = 65;

    private int damage_counter;
    private static int damage_counter_max = 30;

	// Use this for initialization
	void Start () {

        if (Game_Control.control.get_save_data().can_double_jump)
        {
            //doggo is dead
            gameObject.SetActive(false);
            return;
        }


	    //initialize as normal
        attack_mode = false;
        curr_health = max_health;
        dying_counter = 0;
        damage_counter = 0;

        rump.GetComponent<Renderer>().material.color = Color.yellow;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
	}

    public void set_attack_mode(bool b) { attack_mode = b; }
    public void take_damage()
    {
        if (damage_counter > 0 || dying_counter > 0 || curr_health == 0) return; //immune for a short time

        curr_health--;
        if (curr_health == 0)
        {
            //die
            dying_counter = dying_counter_max;
        }
        else
        {
            damage_counter = damage_counter_max;
        }
    }
    public void is_damaged(MessageArgs args)
    {
        args.result = (damage_counter > 0 || dying_counter > 0 || curr_health == 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (dying_counter > 0)
        {
            //dying
            dying_counter--;
            rump.GetComponent<Renderer>().material.color = Color.red;
            body.GetComponent<Renderer>().material.color = Color.red;

            if (dying_counter == 0)
            {
                //dead
                ui_handler.show_doublejump_text();
                Game_Control.control.get_save_data().can_double_jump = true;
                gameObject.SetActive(false);
            }
        }
        else if (damage_counter > 0)
        {
            //taking damage
            damage_counter--;
            rump.GetComponent<Renderer>().material.color = Color.red;

            if (damage_counter == 0)
            {
                //revert back to normal
                rump.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
	}

    void FixedUpdate()
    {
        //movement and stuff

        planet.attract_object(rb);
    }
}