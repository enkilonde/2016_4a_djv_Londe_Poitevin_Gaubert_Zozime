using UnityEngine;
using System.Collections;

public class ActivateOnAwake : MonoBehaviour {

	// Use this for initialization
	void Awake ()
    {
        GetComponent<Renderer>().enabled = true;
	}

    void OnDestroy()
    {
        GetComponent<Renderer>().enabled = false;
    }

}
