using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
            isPaused = !isPaused;

        if(!isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
