using Havir.Sockets.Client;
using Havir.Sockets.Entities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public class RecognizerManager : MonoBehaviour
{

    public bool running;

    private Process serverProcess;
    private SocketClient<ServerActionMessage, UnityActionMessage> client;
    private DialogManager dm;
    private AnimationManager am;
    private Queue<UnityActionMessage> queue;
    private AgentStatusManager currentAgentStatus;
    public GameObject recManagerObject;
    public bool recognizerPaused = false;

    void Start()
    {
        dm = new DialogManager();
        am = new AnimationManager();
        queue = new Queue<UnityActionMessage>();
        recManagerObject = GameObject.FindGameObjectWithTag("Agent");
        currentAgentStatus = recManagerObject.GetComponent<AgentStatusManager>();
        RunGame();
    }

    private void OnDestroy()
    {
        if (serverProcess == null)
            return;
        if (serverProcess.HasExited == false)
            serverProcess.Kill();
        serverProcess.Dispose();
        running = false;
    }

    // Update is called once per frame
    void Update()
    {
        ///Push to talk
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResumeRecognizer();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            PauseRecognizer();
        }
        ///
        if (currentAgentStatus != null && currentAgentStatus.isSpeaking == true) return;
        if (recognizerPaused)
        {
            ResumeRecognizer();
            recognizerPaused = false;
        }
        if (queue.Count == 0) return;
        var nextItem = queue.Dequeue();
        if (nextItem == null) return;
        if (string.IsNullOrWhiteSpace(nextItem.Audio))
            _GenerateSpeech(nextItem.Description);
        else
            _Speech(nextItem.Audio);
        if (nextItem.Animations.Length == 0)
            _GenerateAnimation(nextItem.Description);
        else
            _Animation(nextItem.Animations);
        recognizerPaused = nextItem.Wait;
    }

    private void _Speech(string audio)
    {
        if (string.IsNullOrWhiteSpace(audio)) return;
        var dialog = new Dialog();
        dialog.agent = recManagerObject;
        dialog.audioFileName = audio;
        dm = recManagerObject.GetComponent<DialogManager>();
        dm.Speak(dialog);

    }

    private void _GenerateSpeech(string description)
    {
        ///TODO: Generar audio automático
        _Speech("aGusto");
    }

    private void _Animation(Havir.Sockets.Entities.Animation[] animations)
    {
        if (animations.Length == 0) return;
        am = recManagerObject.GetComponent<AnimationManager>();

        foreach (Havir.Sockets.Entities.Animation a in animations)
        {
            UnityEngine.Debug.Log("ANIMATION:" + a.AnimationName.ToString());
            UnityEngine.Debug.Log("ANIMATION:" + a.StartTime);
            var animate = new Animate();
            animate.animation = a.AnimationName;
            am.PlayAnimation(animate);

        }

    }
    private void _GenerateAnimation(string description)
    {
        ///TODO: Generar animaciónes automáticas
        //_Animation("Gusto");
    }

    void RunGame()
    {
        _StartServer();
        _InitClient();
        PauseRecognizer();
    }

    private void _StartServer()
    {
        ProcessStartInfo info = new ProcessStartInfo();
        //info.WindowStyle = ProcessWindowStyle.Hidden;
        info.FileName = $"{Application.dataPath}/../Havir/Havir.Main.exe";
        serverProcess = new Process();
        serverProcess.StartInfo = info;
        serverProcess.Start();
    }

    private void _InitClient()
    {
        try
        {
            client = new SocketClient<ServerActionMessage, UnityActionMessage>();
            client.Connect(4224);
            var message = JsonConvert.SerializeObject(client);
            client.OnRecivedMessage += OnRecivedMessageHandler;
            Task task = new Task(client.ReceiveDataFromServer);
            task.Start();
            UnityEngine.Debug.Log("Client strated");
        }
        catch (Exception ex)
        {
            throw;
        }
    }


    private void OnRecivedMessageHandler(UnityActionMessage message)
    {
        UnityEngine.Debug.Log("Mensaje recibido");

        if (message.Message != null && message.Message.Equals("##kill"))
        {
            _Interrupt();
            return;
        }
        queue.Enqueue(message);
    }

    private void _Interrupt()
    {
        queue.Clear();
        currentAgentStatus.isSpeaking = false;
    }

    public void PauseRecognizer()
    {
        if (recognizerPaused)
            return;
        var message = new ServerActionMessage();
        message.Pause = true;
        client.SendMessage(message);
    }

    public void ResumeRecognizer()
    {
        if (recognizerPaused)
            return;
        var message = new ServerActionMessage();
        message.Resume = true;
        client.SendMessage(message);
    }
}

