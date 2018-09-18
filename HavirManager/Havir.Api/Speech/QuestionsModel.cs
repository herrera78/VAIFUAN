using Havir.Sockets.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Api.Speech
{
    [Serializable]
    public class Question
    {
        public delegate void QuestionSelected(Question question);
        public QuestionSelected OnQuestionSelected;

        public EmitMessage OnEmitMessage;

        private bool _isRoot;
        private string _id;

        public Question Parent { get; set; }
        public string Id { get { return _id; } }
        public string TargetId { get; set; }
        public string Description { get; set; }
        public string Audio { get; set; }
        public List<Animation> Animations { get; set; }
        public string Keyphrase { get; set; }
        public bool Wait { get; set; }

        public bool IsRoot { get { return _isRoot; } }
        public List<Answer> Answers { get; set; }
        public bool IsRunning { get; set; }

        public Question(string targetId, string id, string keyphrase, string description, string audio,
            string animation, bool isRoot, bool wait)
        {
            TargetId = targetId.Trim();
            _id = id.Trim();
            Description = description.Trim();
            if (audio != null)
                Audio = audio.Trim();
            Animations = new List<Animation>();
            if (string.IsNullOrWhiteSpace(animation) == false)
            {
                Animations.Add(new Animation() { AnimationName = animation });
            }
            if (keyphrase != null)
                Keyphrase = keyphrase.Trim();
            _isRoot = isRoot;
            Wait = wait;
            Answers = new List<Answer>();
        }

        public Question Execute(string keyword, bool kill = true)
        {
            if (kill == true)
                _EmitKillMessage();
            _EmitActionMessage();

            if (Answers.Count == 1 && (Answers.First().Choices.Length == 0 || Answers.First().Choices[0].Trim() == string.Empty))
            {
                var nextQuestion = Answers.Select(x => x.Target).FirstOrDefault();
                if (nextQuestion != null)
                {
                    return nextQuestion.Execute(keyword, false);
                }
            }
            else
            {
                IsRunning = true;
                if (OnQuestionSelected != null)
                    OnQuestionSelected(this);
            }
            return this;
        }

        private void _EmitKillMessage()
        {
            var message = new UnityActionMessage();
            message.Message = "##kill";
            message.MessageType = MessageTypeEnum.Success;
            EmitMessage(message);
        }

        private void _EmitActionMessage()
        {
            var message = new UnityActionMessage();
            message.Description = Description;
            message.Audio = Audio;
            message.Animations = Animations.ToArray();
            message.Wait = Wait;
            message.MessageType = MessageTypeEnum.Success;
            EmitMessage(message);
        }

        public void AddArista(Answer arista)
        {
            //if (Type != NodeType.Decision && _answers.Any())
            //    throw new Exception("Los únicos nodos que permiten múltiples salidas, son los nodos de decisión.");
            Answers.Add(arista);
        }

        public void AddArista(List<Answer> aristas)
        {
            //if (Type != NodeType.Decision && _answers.Any())
            //    throw new Exception("Los únicos nodos que permiten múltiples salidas, son los nodos de decisión.");
            //if (Type != NodeType.Decision && aristas.Count > 1)
            //    throw new Exception("Los únicos nodos que permiten múltiples salidas, son los nodos de decisión.");
            Answers.AddRange(aristas);
        }

        public Answer FindArista(string option)
        {
            return Answers.FirstOrDefault(x => x.Choices.Any(o => o.Equals(option)));
        }

        public void EmitMessage(UnityActionMessage message)
        {
            if (OnEmitMessage != null)
                OnEmitMessage(message);
        }
    }

    [Serializable]
    public class Answer
    {
        public string TargetId { get; set; }
        public Question Target { get; set; }
        public string[] Choices { get; set; }
        public Guid GrammarId { get; set; }
    }
}
