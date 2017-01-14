using UnityEngine;
using System.Collections;

public class Player_Spawner : MonoBehaviour {

    public GameObject spawn_parent;
    public GameObject default_spawn;

    public Vector3 get_location_for_index(int i)
    {
        string check = "Spawn" + i;

        foreach (Transform child in spawn_parent.transform) {
            if (child.name == check) {
                return child.position;
            }
        }
        return default_spawn.transform.position;
    }

    public Attractor get_attractor_for_index(int i)
    {
        string check = "Spawn" + i;

        foreach (Transform child in spawn_parent.transform)
        {
            if (child.name == check)
            {
                return child.GetComponentInChildren<Checkpoint_Platform>().starting_attractor;
            }
        }
        return null;
    }
}
