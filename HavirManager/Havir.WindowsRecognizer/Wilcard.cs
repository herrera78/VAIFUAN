using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;

namespace Havir.WindowsRecognizer
{
    public class Wildcard
    {
        public string WilcardKey { get; set; }
        public string[] Keywords { get; set; }
        public Grammar Grammar { get; internal set; }

        public bool TryGetKeyword(string text, out string keyword)
        {
            for (var i = 0; i < Keywords.Length; i++)
            {
                if (text.IndexOf(Keywords[i]) >= 0)
                {
                    keyword = Keywords[i];
                    return true;
                }

            }
            keyword = null;
            return false;
        }
    }

    public class Anything
    {
        public bool Enabled { get; set; }
        public string AnythingKey { get; set; }
    }
}
