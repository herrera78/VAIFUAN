using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

[XmlRoot("dictionary")]
public class SerializableDictionary<TKey, TValue>
    : Dictionary<TKey, TValue>
{

    #region IXmlSerializable Members
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }

    public static void ReadXml(string filename)
    {
        
    }

    public void WriteXml(string filename)
    {
        XElement xElem = new XElement(
                   "items",
                   this.Select(x => new XElement("item", new XAttribute("id", x.Key), new XAttribute("value", x.Value)))
                );
        xElem.Save(filename);
    }


    #endregion

}