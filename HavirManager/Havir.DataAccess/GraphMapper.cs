using Havir.Api.Speech;
using Havir.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Assets.HAVIR.Scripts.Game.Speech.Graph
{
    public class GraphMapper
    {
        public static List<Question> GetGraphNodes(string graphPath)
        {
            var graph = new List<Question>();
            //var graphMl = MapFromXml();
            var xml = XElement.Load(graphPath);
             
            var graphXml = xml.Elements("{http://graphml.graphdrawing.org/xmlns}graph");
            var keys = xml
                .Elements("{http://graphml.graphdrawing.org/xmlns}key")
                .Where(x => x.Attribute("attr.name") != null);
            var nameId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("NODO"));
            var agentId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("AGENT"));
            var audioId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("AUDIO"));
            var animationId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("ANIM"));
            var keywordId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("KEYWORD"));
            var hvId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("HV"));
            var userId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("USUARIO"));
            var groupId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("GRUPO"));

            if (nameId == null || agentId == null || audioId == null || animationId == null ||
                keywordId == null || hvId == null || hvId == null ||
                userId == null || groupId == null)
                throw new Exception("Los atributos NODO, AGENT, AUDIO, ANIM. KEYWORD, HV, USUARIO Y GRUPO son requeridos.");
            var waitId = keys
                .FirstOrDefault(x => x.Attribute("attr.name").Value.Equals("WAIT"));
            string firstAgent = null;
            foreach (var node in graphXml.Elements("{http://graphml.graphdrawing.org/xmlns}node"))
            {
                var name = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(nameId.Attribute("id").Value));
                var agent = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(agentId.Attribute("id").Value));
                var audio = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(audioId.Attribute("id").Value));
                var animation = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(animationId.Attribute("id").Value));
                var keyword = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(keywordId.Attribute("id").Value));
                var hv = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(hvId.Attribute("id").Value));
                var user = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(userId.Attribute("id").Value));
                var group = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(groupId.Attribute("id").Value));

                if (string.IsNullOrWhiteSpace(firstAgent) && agent != null)
                    firstAgent = agent.Value;
                if (string.IsNullOrWhiteSpace(agent.Value))
                    agent.Value = firstAgent;

                string
                    targetId = node.Attribute("id").Value,
                    id = node.Attribute("id").Value;

                XElement waitElement = null;
                if (waitId != null)
                    waitElement = node
                    .Elements("{http://graphml.graphdrawing.org/xmlns}data")
                    .FirstOrDefault(x => x.Attribute("key").Value.Equals(waitId.Attribute("id").Value));

                bool wait = waitElement == null
                    ? false
                    : waitElement.Value.ToLower().Equals("true");

                var targetEdges = graphXml.Elements("{http://graphml.graphdrawing.org/xmlns}edge").Where(x => x.Attribute("target").Value == id);
                var graphNode = new Question(targetId, id, keyword.Value, hv.Value, audio.Value, animation.Value, targetEdges.Any(), wait);

                graph.Add(graphNode);
            }

            var edges = graphXml.Elements("{http://graphml.graphdrawing.org/xmlns}edge");
            using (var questionDataAccess = new QuestionDataAccess())
            {
                foreach (var edge in edges)
                {
                    var edgeUserData = edge.Elements("{http://graphml.graphdrawing.org/xmlns}data")
                        .FirstOrDefault(x => x.Attribute("key").Value.Equals(userId.Attribute("id").Value));

                    var edgeGroupData = edge.Elements("{http://graphml.graphdrawing.org/xmlns}data")
                        .FirstOrDefault(x => x.Attribute("key").Value.Equals(groupId.Attribute("id").Value));

                    var arista = new Answer() { TargetId = edge.Attribute("target").Value };
                    var choices = new List<string>();

                    if (edgeGroupData != null)
                    {
                        var groupChoises = edgeGroupData.Value.ToLower().Trim().TrimStart(',').TrimEnd(',').Split(',');
                        string tempChoices;
                        foreach (var option in groupChoises)
                        {
                            if (string.IsNullOrWhiteSpace(option))
                                continue;
                            if (questionDataAccess.GetPredefinedAnswers().TryGetValue(option, out tempChoices))
                                choices.AddRange(tempChoices.Split(';'));
                            else
                                Console.WriteLine($"No se encontó el grupo {option} en la arista ");
                        }
                    }
                    if (edgeUserData != null)
                    {
                        choices.AddRange(edgeUserData.Value.ToLower().Trim().TrimStart(';').TrimEnd(';').Split(';'));
                    }

                    arista.Choices = choices.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                    arista.Target = graph.Find(x => x.Id.Equals(arista.TargetId));
                    graph.Find(x => x.TargetId.Equals(edge.Attribute("source").Value)).AddArista(arista);
                }
            }
            return graph;
        }
        

    }
}
