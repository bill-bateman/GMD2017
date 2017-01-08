using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

using UnityEngine.EventSystems;

public class Options_Button : MonoBehaviour
{
    public GameObject event_system_gameobject;
    public EventSystem event_system;

    // Use this for initialization
    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        //close the options scene
        event_system_gameobject.SetActive(false);
        SceneManager.LoadScene("options_menu", LoadSceneMode.Additive);
        Game_Control.control.is_options_menu_open = true;
    }

    void Update()
    {
        //for reactivating the event system after closing Option menu
        if (!event_system_gameobject.activeSelf && !Game_Control.control.is_options_menu_open)
        {
            SceneManager.UnloadScene("options_menu");

            event_system_gameobject.SetActive(true);
            event_system.SetSelectedGameObject(gameObject);
        }
    }
}
