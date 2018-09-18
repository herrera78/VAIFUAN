using Assets.HAVIR.Scripts.Game.Speech;
using Havir.Api.Speech;
using Havir.DataAccess;
using Havir.Sockets.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Manager
{ 
    public class ScriptManager
    {
        public EmitMessage OnEmitMessage;

        private static List<Question> _graph;
        private IVoiceRecognizer _recognizer;
        private Question _currentQuestion;

        public ScriptManager(IVoiceRecognizer recognizer)
        {
            this._recognizer = recognizer;
            _recognizer.OnKeywordRecognized += OnKeywordRecognizedHandler;
        }

        public void Init()
        {
            _LoadGraphToRecognizerManager();
        }

        private void _LoadGraphToRecognizerManager()
        {
            ///Carga el grafo que tiene el gión a seguir
            ///
            using (var dataAccess = new QuestionDataAccess())
            {
                _graph = dataAccess.GetCurrentGraph();
                //for(int i =0; i<_graph.Count; i++)
                foreach (var node in _graph)
                {
                    node.OnEmitMessage += EmitQuestionMessage;
                    node.OnQuestionSelected += OnQuestionSelectedHandler;
                    if (string.IsNullOrWhiteSpace(node.Keyphrase) == false)
                    {
                        _recognizer.StartKeywordRecognition(AddKeywordRecognition(
                            string.Format("#node|{0}", node.Id),
                            node.Keyphrase.Split(',')));
                    }
                    var answers = node.Answers;
                    foreach (var arista in answers)
                    {
                        if (arista.Choices == null || arista.Choices.Any() == false || string.IsNullOrWhiteSpace(arista.Choices[0]))
                            continue;

                        if (arista.Choices.Length == 1 && arista.Choices[0].ToLower() == "*")
                        {
                            arista.GrammarId = AddWildcardRecognition(
                               string.Format("#node{0}|{1}", node.Id, arista.TargetId));
                        }
                        else
                            arista.GrammarId = AddKeywordRecognition(
                                string.Format("#node{0}|{1}", node.Id, arista.TargetId),
                                arista.Choices.ToArray());
                    }
                    node.Answers = answers;
                }
            }
            ///Agrega frases claves para inicial el guión
            ///
            _recognizer.StartKeywordRecognition(AddKeywordRecognition("#start", new string[] { "Puede comenzar con la entrevista.", "iniciar guión", "pregunteme" }));
        }

        private void OnQuestionSelectedHandler(Question question)
        {
            foreach (var arista in question.Answers)
            {
                _recognizer.StartKeywordRecognition(arista.GrammarId);
            }
        }
        /// <summary>
        /// Agrega palabras clave 
        /// </summary>
        /// <param name="semanticKey">Clave de identificación</param>
        /// <param name="keywords">Palabras clave</param>
        /// <returns></returns>
        private Guid AddWildcardRecognition(string semanticKey)
        {
            var id = _recognizer.AddSemanticRecognition(semanticKey, null);
            return id;
        }

        /// <summary>
        /// Agrega palabras clave 
        /// </summary>
        /// <param name="semanticKey">Clave de identificación</param>
        /// <param name="keywords">Palabras clave</param>
        /// <returns></returns>
        private Guid AddKeywordRecognition(string semanticKey, string[] keywords)
        {
            var id = _recognizer.AddSemanticRecognition(semanticKey, keywords);
            return id;
        }


        private void OnKeywordRecognizedHandler(KeywordRecognizedArgs args)
        {
            if (args.SemanticKey.Equals("#start"))
                _ExecuteStart(args.Text);
            else if (args.SemanticKey.StartsWith("#node"))
                _ExecuteQuestion(args.SemanticKey, args.Text);
        }


        private void _ExecuteQuestion(string semanticKey, string text)
        {
            try
            {
                var nodes = semanticKey.Replace("#node", "").Split('|');
                if (nodes.Length < 2)
                    return;
                //var currentQuestion = _graph
                //    .FirstOrDefault(x => x.Id.Equals(nodes[0]));
                var nextQuestion = _graph
                    .FirstOrDefault(x => x.Id.Equals(nodes[1]));
                if (nextQuestion == null)
                    return;
                if (_currentQuestion != null)
                {
                    foreach (var arista in _currentQuestion.Answers)
                        _recognizer.StopKeywordRecognition(arista.GrammarId);
                }
                Debug.WriteLine("Ejecutando handler: " + semanticKey + " Text: " + text);
                _currentQuestion = nextQuestion.Execute(text);

            }
            catch (Exception ex)
            {
                Debug.Fail("Error en e handler de la pregunta", ex.ToString());
            }
        }

        private void _ExecuteStart(string keyword)
        {
            //for (int i = 0; i < _graph.Count; i++)
            //{
            //    _graph[i].IsRunning = false;
            //}
            //var startNode = _graph.Where(x => x.Type == NodeType.Start).FirstOrDefault();
            //_currentQuestion = startNode.Execute(keyword);
        }

        public void EmitQuestionMessage(UnityActionMessage message)
        {
            OnEmitMessage(message);
        }
    }
}
