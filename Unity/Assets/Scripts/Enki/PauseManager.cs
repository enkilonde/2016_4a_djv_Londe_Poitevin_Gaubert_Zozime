using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour
{

    public Canvas pauseCanvas;

    public bool paused = false;

	void Start ()
    {
	
	}
	
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerPause();
        }

	}

    public void TriggerPause()
    {
        pauseCanvas.enabled = !pauseCanvas.enabled;
        paused = pauseCanvas.enabled;
    }
}
