using UnityEngine;
using System.Collections;

public class DogCage_Switch : MonoBehaviour {

    public GameObject cage;
    public DogBehaviour dog;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            cage.SetActive(false);
            dog.set_attack_mode(true);
        }
    }
}
