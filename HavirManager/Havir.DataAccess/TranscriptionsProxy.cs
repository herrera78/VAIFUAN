using Assets.HAVIR.Scripts.Game.Speech;
using Assets.HAVIR.Scripts.Game.Speech.Graph;
using Havir.Api.Speech;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Havir.DataAccess
{
    public class TranscriptionsProxy
    {
        string apiUrl= "http://gentle-demo.lowerquality.com/transcriptions?async=false";
        public async Task<string> Transcript(string audio, string text)
        {
            //using (var audioStream = File.Open(Path.Combine(@"Assets\Resources\_Dialogs", audio), FileMode.Open)) 
            using (var audioStream = File.Open(Path.Combine(@"../Assets/Resources/_Dialogs", audio), FileMode.Open))
            //using (MemoryStream textStream = _GetStream(text))
            using (HttpClient client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(audioStream), "audio");
                content.Add(new StringContent(text), "transcript");
                var response = await client.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Pailas.. " + response.ReasonPhrase);
                else
                    return await response.Content.ReadAsStringAsync();
            }
        }

        private MemoryStream _GetStream(string text)
        {
            MemoryStream ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            try
            {
                sw.Write(text);
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
            }
            finally
            {
                sw.Dispose();
            }
            return ms;
        }
    }
}
