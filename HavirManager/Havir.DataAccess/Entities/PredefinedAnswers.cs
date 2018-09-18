using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Assets.HAVIR.Scripts.Game.Speech
{
    public static class PredefinedAnswers
    {

        static Dictionary<string, string> _dictionary;
        

        //private static Dictionary<string, string> _dictionary = new Dictionary<string, string>()
        //{
        //    { "verdadero", "si;claro;desde luego;por supuesto;bueno;si senior" } ,
        //    { "falso", "no;ne;no me importa;claro que no;no senior" },
        //    { "true", "si;claro;desde luego;por supuesto;bueno;si senior" } ,
        //    { "false",  "no;ne;no me importa;claro que no;no senior" },
        //    { "si",  "si;claro;desde luego;por supuesto;bueno;si senior" } ,
        //    { "no",  "no;ne;no me importa;claro que no;no senior" },
        //    { "circunvalar", "circunvalar" },
        //    { "sur", "sur"},
        //    { "iberica", "iberica" },
        //    { "federman", "federman" },
        //};

    //    public static Dictionary<string, string> GetPredefinedAnswers()
    //    {
    //        //if (false)
    //        {   
    //            //using (var xwriter = new XmlTextWriter(@"C:\Users\Lucho\Documents\PredefinedAnswers.xml", System.Text.Encoding.UTF8))
    //            //{
    //            //    xwriter.WriteStartDocument(true);
    //            //    _dictionary.WriteXml(xwriter);
    //            //    xwriter.WriteEndDocument();
    //            //    xwriter.Close();
    //            //}

    //        }

    //        return _dictionary;
    //    }
    }
}
