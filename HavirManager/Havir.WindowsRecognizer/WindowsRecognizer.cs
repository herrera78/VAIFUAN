using Havir.Api.Log;
using Havir.Api.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Havir.WindowsRecognizer
{

    public class WindowsRecognizer : IVoiceRecognizer
    {
        SpeechRecognitionEngine _recognizer; 

        public event KeyWordRecognized OnKeywordRecognized;
        public event WilcardRecognized OnWilcardRecognized;

        public Dictionary<Guid, Grammar> semantics;
        public Dictionary<Guid, Wildcard> _wildcards;
        public Dictionary<Guid, Anything> _anything;

        public WindowsRecognizer()
        {
            semantics = new Dictionary<Guid, Grammar>();
            _wildcards = new Dictionary<Guid, Wildcard>();
            _anything = new Dictionary<Guid, Anything>();
            _InitRecognizer();
        }

        private void _InitRecognizer()
        {
            _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("es-CO"));
            _recognizer.BabbleTimeout = TimeSpan.FromSeconds(30);
            _recognizer.LoadGrammar(new DictationGrammar());
            ///Register a handler for the SpeechRecognized event.
            ///
            _recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(OnSpeechRecognized);
            _recognizer.LoadGrammarCompleted += grammarLoadded;
            //_recognizer.SpeechRecognized += WildcardInputHandler;
            _recognizer.SpeechDetected += OnSpeechDetectedHandler;
            _recognizer.AudioSignalProblemOccurred += AudioSignalProblemOccurredHandler;
            _recognizer.RecognizeCompleted += RecognizedCompleted;
            _recognizer.AudioStateChanged += OnAudioStateChange;
            _recognizer.SpeechHypothesized += Hipothesized;

        }

        private void Hipothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Logger.Write("Hipotesis." + e.Result.Text);
        }

        private void OnAudioStateChange(object sender, AudioStateChangedEventArgs e)
        {
            Logger.Write("Cambio de estado." + e.AudioState);
        }

        private void RecognizedCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Logger.Write("Reconocimiento finalizado.");
        }

        private void grammarLoadded(object sender, LoadGrammarCompletedEventArgs e)
        {
            Logger.Write("Gramatica cargada.");
        }

        private void AudioSignalProblemOccurredHandler(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            //Logger.Debug("Problema detectado con la señal del audio" + e.AudioSignalProblem);
        }

        private void OnSpeechDetectedHandler(object sender, SpeechDetectedEventArgs e)
        {
            Logger.Debug("Escuchando "+ e.AudioPosition);
        }

        private void WildcardInputHandler(object sender, SpeechRecognizedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Result.Text))
                return;
            Logger.Debug("Reconocido: " + e.Result.Text);
            string keyword;
            foreach (var wildcard in _wildcards)
            {
                if (wildcard.Value.TryGetKeyword(e.Result.Text, out keyword))
                {
                    if (OnWilcardRecognized != null)
                        OnWilcardRecognized(new WilcardRecognizedArgs(wildcard.Value.WilcardKey, keyword, e.Result.Text));

                }
            }
        }

        private void OnSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Logger.Debug("Reconocido: "+e.Result.Text);
            if (e.Result.Semantics.Any())
            {
                foreach (var semantic in e.Result.Semantics)
                {
                    OnKeywordRecognized(new KeywordRecognizedArgs(semantic.Key, e.Result.Text));
                }
            }
            else
            {
                foreach (var anything in _anything.Where(x => x.Value.Enabled))
                {
                    OnKeywordRecognized(new KeywordRecognizedArgs(anything.Value.AnythingKey, e.Result.Text));
                }
            }
        }

        public Guid AddSemanticRecognition(string semanticKey, string[] keywords)
        {
            if (keywords == null)
                return _AddAnything(semanticKey);
            if (keywords.Length == 1 && keywords[0].Trim().Equals(string.Empty))
                return default(Guid);
            Choices choices = new Choices(keywords);
            SemanticResultKey semanticResultKey = new SemanticResultKey(semanticKey, choices);
            GrammarBuilder grammarBuilder = semanticResultKey.ToGrammarBuilder();
            Grammar grammar = new Grammar(grammarBuilder);
            var id = Guid.NewGuid();
            grammar.Name = id.ToString();
            grammar.Enabled = false;
            grammar.Priority = 120;
            _recognizer.LoadGrammar(grammar);
            semantics.Add(id, grammar);
            return id;
        }

        private Guid _AddAnything(string semanticKey)
        {
            var id = Guid.NewGuid();
            _anything.Add(id, new Anything() { AnythingKey = semanticKey });
            return id;
        }

        public Guid AddKeywordRecognition(string[] keywords)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _recognizer.Dispose();
        }

        public void InitRecognizer(bool dictation, bool keywordRecognation)
        {
            // Configure input to the speech recognizer.
            _recognizer.SetInputToDefaultAudioDevice();

            // Start asynchronous, continuous speech recognition.
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void RemoveKeywordRecognition(Guid id)
        {
            Grammar grammar;
            if (semantics.TryGetValue(id, out grammar))
            {
                _recognizer.UnloadGrammar(grammar);
                semantics.Remove(id);
            }

            Anything anything;
            if (_anything.TryGetValue(id, out anything))
                _anything.Remove(id);
        }

        public void StopKeywordRecognition(Guid id)
        {
            Grammar grammar;
            if (semantics.TryGetValue(id, out grammar))
                grammar.Enabled = false;

            Anything anything;
            if (_anything.TryGetValue(id, out anything))
                anything.Enabled = false;
        }


        public void StartKeywordRecognition(Guid id)
        {
            Grammar grammar;
            if (semantics.TryGetValue(id, out grammar))
                grammar.Enabled = true;

            Anything anything;
            if (_anything.TryGetValue(id, out anything))
                anything.Enabled = true;
        }
        public Guid AddWildcardGrammar(string semanticKey, string[] keywords)
        {
            Choices choices = new Choices(keywords);
            GrammarBuilder wilcardGrammarBuilder = new GrammarBuilder();
            wilcardGrammarBuilder.Append(choices);
            wilcardGrammarBuilder.AppendWildcard();
            SemanticResultKey semanticResultKey = new SemanticResultKey(semanticKey, choices);
            var id = Guid.NewGuid();
            Grammar grammar = new Grammar(wilcardGrammarBuilder);
            grammar.Name = id.ToString();
            _recognizer.LoadGrammar(grammar);
            _wildcards.Add(id, new Wildcard() { WilcardKey = semanticKey, Keywords = keywords, Grammar = grammar });
            return id;
        }

        public void PauseKeywordRecognition()
        {
            _recognizer.RecognizeAsyncStop();
            Console.WriteLine("Recognizer manager paused.");
        }

    }
}