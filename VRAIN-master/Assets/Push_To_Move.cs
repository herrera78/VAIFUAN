using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push_To_Move : MonoBehaviour {


    private DialogManager dm;
    private AnimationManager am;
    public GameObject[] recManagerObjects;
    public int agente;

    // Use this for initialization
    void Start () {
        dm = new DialogManager();
        am = new AnimationManager();
        recManagerObjects = GameObject.FindGameObjectsWithTag("Agent");
        agente = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            agente = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            agente = 1;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _Speech("aHola", agente);
            _Animation("Hola", agente);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            _Speech("aGusto", agente);
            _Animation("Gusto", agente);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _Speech("aBien", agente);
            _Animation("Bien", agente);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            _Speech("aRanking", agente);
            _Animation("Ranking", agente);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            _Speech("aSur", agente);
            _Animation("Sur", agente);
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            _Speech("aCircunvalar", agente);
            _Animation("Circunvalar", agente);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            _Speech("aFederman", agente);
            _Animation("Federman", agente);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            _Speech("aUsme", agente);
            _Animation("Usme", agente);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            _Speech("aIberica", agente);
            _Animation("Iberica", agente);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            _Speech("aBeca", agente);
            _Animation("Beca", agente);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _Speech("aConsultar", agente);
            _Animation("Consultar", agente);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void _Speech(string audio, int agente)
    {
        
        if (string.IsNullOrWhiteSpace(audio)) return;
        var dialog = new Dialog();
        dialog.agent = recManagerObjects[agente];
        dialog.audioFileName = audio;
        dm = recManagerObjects[agente].GetComponent<DialogManager>();
        dm.Speak(dialog);
    }

    private void _Animation(string animations, int agente)
    {
        if (string.IsNullOrWhiteSpace(animations)) return;
        var animate = new Animate();
        animate.animation = animations;
        am = recManagerObjects[agente].GetComponent<AnimationManager>();
        am.PlayAnimation(animate);
    }


}
