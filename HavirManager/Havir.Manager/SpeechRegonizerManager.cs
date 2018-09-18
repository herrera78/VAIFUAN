using Assets.HAVIR.Scripts.Game.Speech;
using Havir.Api.Speech;
using Havir.DataAccess;
using Havir.Sockets.Entities;
using Havir.WindowsRecognizer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Havir.Manager
{
    public class SpeechRegonizerManager : IDisposable
    { 
        public EmitMessage OnEmitMessage; 
          
        private static IVoiceRecognizer _recognizer;
        private static IVoiceRecognizer Recognizer
        {
            get
            {
                if (_recognizer == null)
                    _recognizer = new WindowsRecognizer.WindowsRecognizer();
                return _recognizer;
            }
        }
        public ScriptManager _scriptManager = new ScriptManager(Recognizer);
        private bool _dictation;
        private bool _keywordRecognation;


        public void Dispose()
        {
            Recognizer.Dispose();
        }

        #region IVoiceRecognizer actions


        public void InitRecognizer(bool dictation, bool keywordRecognation)
        {
            _keywordRecognation = keywordRecognation;
            _dictation = dictation; //Typo? Dictation?
            _scriptManager.OnEmitMessage += EmitQuestionMessage; 
            _scriptManager.Init();

            //Recognizer.AddWildcardGrammar("Pregunta", new string[] { "quiero saber", "Deseo saber de", "quiero saber de", "hábleme de " });
            Recognizer.OnWilcardRecognized += OnWilcardRecognizedHandler;

            Recognizer.InitRecognizer(dictation, keywordRecognation);
        }
        
        private void OnWilcardRecognizedHandler(KeywordRecognizedArgs args)
        {
            Debug.Write("Wilcard reconocido: " + args.Text);
        }

        public void RemoveKeywordRecognition(Guid id)
        {
            Recognizer.RemoveKeywordRecognition(id);
        }

        public void StopKeywordRecognition(Guid id)
        {
            Recognizer.StopKeywordRecognition(id);
        }

        public void Pause()
        {
            Recognizer.PauseKeywordRecognition();
        }


        public void Resume()
        {
            Recognizer.InitRecognizer(_dictation, _keywordRecognation);
        }


        public void EmitQuestionMessage(UnityActionMessage message)
        {
            if (message.Wait)
                Pause();
            OnEmitMessage(message);
        } 
        #endregion
    }
}