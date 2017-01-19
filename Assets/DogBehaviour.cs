using UnityEngine;
using System.Collections;

public class DogBehaviour : MonoBehaviour {

    public UI_Handler ui_handler;
    public GameObject dog;
    public Attractor planet;
    public GameObject player;

    public AudioSource audio_song;
    public AudioClip audio_boss_song;
    public AudioClip audio_normal_song;

    public float overshoot_amount = 10.0f;
    public float speed = 2.0f;

    public AudioClip dog_bork;
    public AudioClip dog_squeal;
    public AudioClip dog_growl;

    private Rigidbody rb;

    private bool attack_mode;
    private int attack_state;
    private Vector3 attacking_pos;
    private bool has_looked;

    private Quaternion old, attack_rot;
    private float counter_float;
    private static float counter_float_incr = 0.1f;

    private int curr_health;
    private static int max_health = 3;

    private int dying_counter;
    private static int dying_counter_max = 65;

    private int damage_counter;
    private static int damage_counter_max = 30;

    private AudioSource audio_dog;

	// Use this for initialization
	void Start () {

        if (Game_Control.control.get_save_data().can_double_jump)
        {
            //doggo is dead
            gameObject.SetActive(false);
            return;
        }

        audio_dog = GetComponent<AudioSource>();

	    //initialize as normal
        attack_mode = false;
        attack_state = 0;

        curr_health = max_health;
        dying_counter = 0;
        damage_counter = 0;

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
	}

    public void set_attack_mode(bool b) { 
        attack_mode = b;
        if (attack_mode)
        {
            audio_dog.clip = dog_growl;
            audio_dog.Play();

            audio_song.clip = audio_boss_song;
            audio_song.Play();
        }
    }
    public void take_damage()
    {
        if (damage_counter > 0 || dying_counter > 0 || curr_health == 0) return; //immune for a short time

        curr_health--;
        audio_dog.clip = dog_squeal;
        audio_dog.Play();

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
            dog.GetComponent<Renderer>().material.color = Color.red;

            if (dying_counter == 0)
            {
                //dead
                ui_handler.show_doublejump_text();
                Game_Control.control.get_save_data().can_double_jump = true;
                gameObject.SetActive(false);

                audio_song.clip = audio_normal_song;
                audio_song.Play();
            }
        }
        else if (damage_counter > 0)
        {
            //taking damage
            damage_counter--;
            dog.GetComponent<Renderer>().material.color = Color.yellow;

            if (damage_counter == 0)
            {
                //revert back to normal
                dog.GetComponent<Renderer>().material.color = Color.white;

                audio_dog.clip = dog_bork;
                audio_dog.Play();
            }
        }
	}

    void FixedUpdate()
    {
        //movement and stuff
        if (attack_mode && curr_health > 0)
        {
            //attack
            if (attack_state == 0)
            {
                //get player's current position
                attacking_pos = player.transform.position;
                Vector3 curr_pos = transform.position;

                //we want to get a point that is past the player's current position (i.e. we want the dog to overshoot)
                Vector3 dir = Vector3.Normalize(attacking_pos - curr_pos); //get direction of dog to player
                dir *= overshoot_amount;
                attacking_pos += dir;

                Vector3 up = Vector3.Normalize(transform.position - planet.transform.position);

                old = transform.rotation;
                attack_rot = Quaternion.LookRotation(dir, up);
                 
                counter_float = 0;
                attack_state++;

                audio_dog.clip = dog_bork;
                audio_dog.Play();
            }
            else if (attack_state == 1)
            {
                //look at player
                transform.rotation = Quaternion.Slerp(old, attack_rot, counter_float);
                counter_float += counter_float_incr * Time.timeScale;

                if (counter_float >= 1)
                {
                    attack_state++;
                }
                has_looked = false;
            }
            else if (attack_state == 2)
            {
                //move until we get to attack_pos
                //get direction we should go in
                Vector3 dir = (attacking_pos - transform.position);
                Vector3 movement = Vector3.Project(dir, transform.forward);
                dir = Vector3.Normalize(movement);

                transform.position += speed * dir;

                if (!has_looked && Random.Range(0.0f, 1.0f) < 0.01f)
                {
                    Vector3 up = Vector3.Normalize(transform.position - planet.transform.position);
                    transform.LookAt(player.transform.position, up);
                    has_looked = true;

                    audio_dog.clip = dog_growl;
                    audio_dog.Play();
                }

                if (Vector3.Magnitude(movement) < 2.0f)
                {
                    attack_state = 0;
                }
            }
        }
        if (!attack_mode && player.GetComponent<Player_Movement>().is_on_planet3())
        {
            if (Random.Range(0.0f, 1.0f) < 0.01f)
            {
                audio_dog.clip = dog_bork;
                audio_dog.Play();
            }
            Vector3 up = Vector3.Normalize(transform.position - planet.transform.position);
            transform.LookAt(player.transform.position, up);
        }

        planet.attract_object(rb);
    }
}