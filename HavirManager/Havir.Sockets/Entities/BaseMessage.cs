using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Sockets.Entities
{
    public class BaseMessage
    {
        public MessageTypeEnum MessageType { get; set; }
        public string Message { get; set; }
    }
}
