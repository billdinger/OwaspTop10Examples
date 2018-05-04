using System;
using System.IO;
using System.Xml;

namespace Blog.XML
{
    public class XmlDataImport
    {

        public string ImportBlogEntries(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            string xml = "<?xml version=\"1.0\" ?><!DOCTYPE doc" +
                            "[<!ENTITY win SYSTEM \"file:///C:/Users/bdinger/Documents/test.txt\">]><doc> &win;</doc> ";
            XmlTextReader myReader = new XmlTextReader(new StringReader(xml));

            var result = string.Empty;
            while (myReader.Read())
            {
                if (myReader.NodeType == XmlNodeType.Element)
                {
                    result = myReader.ReadElementContentAsString();
                }
            }
            return result;
        }
    }
}