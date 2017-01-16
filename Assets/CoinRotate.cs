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

    public int coin_index = -1; //NEEDS TO BE UNIQUE ACROSS ALL COINS

    private float angle;
    private float angle_incr;

    private int animation_count;
    private static int animation_max = 100;
    private float up_incr;

	// Use this for initialization
	void Start () {
        if (Game_Control.control.check_cat_coin(coin_index))
        {
            //coin has already been collected
            //don't spawn
            gameObject.SetActive(false);
        }
        angle_incr = 1.0f;
        animation_count = 0;
        up_incr = 0.01f;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeScale == 0) return;

        if (animation_count == 0)
        {
            //rotate around the up axis
            gameObject.transform.Rotate(gameObject.transform.forward, angle_incr, Space.World);
        }
        if (animation_count > 0)
       {
           //rotate faster
           gameObject.transform.Rotate(gameObject.transform.forward, animation_count * angle_incr, Space.World);
           //move up
           gameObject.transform.position -= (up_incr) * gameObject.transform.forward;

           animation_count++;

           if (animation_count > animation_max)
           {
               gameObject.SetActive(false);
           }
       }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player" && animation_count == 0)
        {
            animation_count = 1; //start the animation
            Game_Control.control.collect_cat_coin(coin_index); //increase by 1
            other.GetComponentInParent<Player_Movement>().ui_handler.show_save_text();
        }
    }
}
