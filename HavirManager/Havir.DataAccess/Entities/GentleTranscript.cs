using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havir.DataAccess.Entities
{
    public class GentleTranscript
    {
        public string Transcript { get; set; }
        public WordTranscript[] Words { get; set; }
    }
    public class WordTranscript
    {
        public string Word { get; set; }
        public decimal Start { get; set; }
        public string Case { get; set; }
    }
}
