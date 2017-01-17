using UnityEngine;
using System.Collections;

public class EnemyDamage : MonoBehaviour {

    public Player_Movement player;

    private GameObject enemy;

	// Use this for initialization
	void Start () {
        enemy = transform.parent.gameObject;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            //if player has not recently been hurt
            if (player.did_just_take_damage())
            {
                //can't take damage
            }
            else
            {
                //take damage
                enemy.SendMessage("take_damage");
            }
        }
    }
}
