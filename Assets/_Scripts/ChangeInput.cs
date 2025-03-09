using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeInput : MonoBehaviour
{

    EventSystem system;

    public Button submitBotton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            Selectable previous = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (previous != null)
            {
                previous.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            submitBotton.onClick.Invoke();
            Debug.Log("Button pressed");
        }

    }
}
