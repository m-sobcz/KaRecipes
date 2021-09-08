using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace KaRecipes.BL.Utils
{
    internal static class ParseHelpers
    {

        public static Stream ToStream(this string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        public static T ParseXML<T>(this string content) where T : class
        {
            var reader = XmlReader.Create(content.Trim().ToStream(), new XmlReaderSettings());
            XmlRootAttribute xRoot = new();
            xRoot.ElementName = "Parameters";
            // xRoot.Namespace = "http://www.cpandl.com";
            xRoot.IsNullable = true;
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }

    }
}