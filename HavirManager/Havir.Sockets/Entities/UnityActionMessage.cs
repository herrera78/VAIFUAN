using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Sockets.Entities
{
    [Serializable]
    public class UnityActionMessage : BaseMessage
    {
        public string Description { get; set; }
        public Animation[] Animations { get; set; }
        public string Audio { get; set; }
        public string Agent { get; set; }
        public bool Wait { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Description == null)
                return Message;
            sb.AppendLine(string.Format("Description: {0}", Description));
            sb.AppendLine(string.Format("Animation: {0}", string.Join(",",Animations.Select(x=>x.AnimationName + ": "+ x.StartTime))));
            sb.AppendLine(string.Format("Audio: {0}", Audio));
            return sb.ToString();
        }
    }

    [Serializable]
    public class Animation
    {
        public string AnimationName { get; set; }
        public decimal StartTime { get; set; }

    }
}
