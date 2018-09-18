using Havir.Api.Speech;
using Havir.DataAccess;
using Havir.DataAccess.Entities;
using Havir.Sockets.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Havir.GraphProcessor
{
    public class GraphProcessor
    {
        public async Task Run()
        {
            QuestionDataAccess questionDataAccess = new QuestionDataAccess();
            var questions = questionDataAccess.GetAll("../Graph/dialogo ejemplo.graphml");
            TranscriptionsProxy proxy = new TranscriptionsProxy();
            var keywords = questionDataAccess.GetKeyWords();
            Trace.WriteLine($"Inició la carga de los nodos.");
            foreach (var question in questions.Where(x => x.Animations.Any() == false))
            {
                Trace.WriteLine($"Procesando nodo: {question.Description}");
                var json = await proxy.Transcript($"{question.Audio}.mp3", question.Description);
                GentleTranscript response = JsonConvert.DeserializeObject<GentleTranscript>(json);
                foreach (var word in response.Words.Where(x => x.Case.ToLower().Equals("success")))
                {
                    var rule = keywords.rule.Where(x => x.pattern.Any(p => p.Value.ToLowerInvariant().Equals(word.Word.ToLowerInvariant()))).FirstOrDefault();
                    if (rule == null)
                        continue;
                    question.Animations.Add(new Animation() { AnimationName = rule.animation, StartTime = word.Start });

                }
            }
            Trace.WriteLine($"Finalizó la carga de los nodos.");
            await SaveGraph(questions);
            Trace.WriteLine($"Se almacenó el grafo serializado correctamente.");

        }

        public async Task SaveGraph(List<Question> graph)
        {
            await Task.Run(() =>
            {
                IFormatter formatter = new BinaryFormatter();
                using (var stream = new FileStream("../Graph/data.hv", FileMode.Create, FileAccess.Write, FileShare.None))
                    formatter.Serialize(stream, graph);
            });
        }
    }
}
