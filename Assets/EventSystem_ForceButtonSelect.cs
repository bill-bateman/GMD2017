using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class EventSystem_ForceButtonSelect : MonoBehaviour {
    public EventSystem e;
    	
	// Update is called once per frame
	void Update () {
        if (e.currentSelectedGameObject == null)
        {
            e.SetSelectedGameObject(e.firstSelectedGameObject);
        }
	}
}
