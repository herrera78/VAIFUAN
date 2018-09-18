using Assets.HAVIR.Scripts.Game.Speech;
using Assets.HAVIR.Scripts.Game.Speech.Graph;
using Havir.Api.Speech;
using Havir.DataAccess.Entities;
using Havir.Sockets.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Havir.DataAccess
{
    public class QuestionDataAccess : IDisposable
    {
        Dictionary<string, string> _pedefinedAnswers;
        public QuestionDataAccess(string answersPath = "Graph/PredefinedAnswers.xml")
        {
            XElement xElem2 = XElement.Load(answersPath);
            _pedefinedAnswers = xElem2.Descendants("item")
                                .ToDictionary(x => (string)x.Attribute("id"), x => (string)x.Attribute("value"));
        }

        public void Dispose()
        {
        }

        public List<Question> GetCurrentGraph(string graphPath = "Graph/data.hv")
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new FileStream(graphPath, FileMode.Open, FileAccess.Read, FileShare.None))
                if (stream.CanRead == false)
                    return GetAll();
                else
                    return (List<Question>)formatter.Deserialize(stream);
        }

        public List<Question> GetAll(string graphPath = "Graph/dialogo ejemplo.graphml")
        {
            var graph = GraphMapper.GetGraphNodes(graphPath);
            return graph;
        }

        public Dictionary<string, string> GetPredefinedAnswers()
        {
            return _pedefinedAnswers;
        }

        public Words GetKeyWords()
        {
            using (StreamReader reader = new StreamReader("Graph/lexico.xml", Encoding.UTF8, true))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Words));

                // Deserialize method
                var currentWords = (Words)serializer.Deserialize(reader);
                return currentWords;
            }
        }

    }
}
