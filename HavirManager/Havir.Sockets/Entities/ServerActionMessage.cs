using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Sockets.Entities
{
    public class ServerActionMessage : BaseMessage
    {
        public bool Pause { get; set; }
        public bool Resume { get; set; }
    }
}
