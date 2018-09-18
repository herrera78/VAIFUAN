using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushToTalk : MonoBehaviour {
    public GameObject recManagerObject;
    public RecognizerManager recognizerManager;
	// Use this for initialization
	void Start () {
        recManagerObject = GameObject.FindGameObjectWithTag("Agent");
        // If multiple agents, use "FindGameObjectSWithTag instead.
        // PLace objects in array, and enable and disable all elements through a loop
        // the same way.
        recognizerManager = recManagerObject.GetComponent<RecognizerManager>();
        Invoke("EnableToggleTalking", 1f);
	}

    void EnableToggleTalking()
    {
        recognizerManager.enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTalking();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            Invoke("ToggleTalking",1f);
        }
	}

    public void ToggleTalking()
    {
        if(!recognizerManager.enabled) 
        {
            recognizerManager.enabled = true;
        }
        else
        {
            recognizerManager.enabled = false;
        }
        Debug.Log("Can talk: " + recognizerManager.enabled);
    }
}
