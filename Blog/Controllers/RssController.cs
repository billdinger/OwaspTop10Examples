using System.IO;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class RssController : Controller
    {

        [HttpGet]
        public IActionResult Feed()
        {

            // Example of XML which is vulnerable.
            string xml = "<?xml version=\"1.0\" ?><!DOCTYPE doc" +
                         "[<!ENTITY oops SYSTEM \"file:///C:/Users/bdinger/Documents/test.txt\">]><doc> &win;</doc> ";

            XmlTextReader xmlReader = new XmlTextReader(new StringReader(xml));
            xmlReader.DtdProcessing = DtdProcessing.Parse;
            var result = string.Empty;
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    result = xmlReader.ReadElementContentAsString();
                }
            }

            return Ok(result);
        }
    }
}
