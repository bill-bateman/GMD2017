using UnityEngine;
using System.Collections;

public class DogCage_Switch : MonoBehaviour {

    public GameObject cage;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            cage.SetActive(false);
        }
    }
}
