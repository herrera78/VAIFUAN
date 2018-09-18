using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.Api.Speech
{
    public class Animate
    {
        public Animate(string agent, string animation)
        {
            this.animation = animation;
            this.agent = agent;
        }

        public string animation;

        public string agent;

        public bool loop = false;

        public float timeout = 0;

        public float delay = 0;

        public bool toIdle = true;
    }
}
