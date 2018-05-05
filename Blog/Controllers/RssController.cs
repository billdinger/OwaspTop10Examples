using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blog.Controllers
{
    public class RssController : Controller
    {
        private readonly FeedContext _context;

        public RssController(FeedContext context)
        {
            _context = context;
        }

        /// <summary>
        /// A6 - Insecure Deserialization  - as this takes arbitrary content with typenameHandling set to auto
        /// it'll allow to you instantiate all sorts of random payloads - such as System.Io.FileINfo which would allow you to read
        /// arbitrary date on the host computer. See https://www.alphabot.com/security/blog/2017/net/How-to-configure-Json.NET-to-create-a-vulnerable-web-API.html 
        /// for the original attack.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateFeed()
        {
            string content = string.Empty;
            using (Stream receiveStream = HttpContext.Request.Body)
            {
                using (StreamReader reader = new StreamReader(receiveStream))
                {
                    content = reader.ReadToEnd();
                }
            }

            var entry = JsonConvert.DeserializeObject<Feed>(content, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto // A6 - Insecure Deserailization - You shoudl instead use TypeNameHandling.None
            });

            _context.Add(entry);
            await _context.SaveChangesAsync();

            return Ok();

        }

        /// <summary>
        /// A4 - XML External Entities (XXE) - This allows an external XML entity to be read and parsed as its default
        /// behavior is set to parse, allowing arbitrary fishing of information.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult LoadFeed()
        {

            // Example of XML which is vulnerable.
            string xml = "<?xml version=\"1.0\" encoding=\"ISO - 8859 - 1\"?>" +
                "<!DOCTYPE foo [" +
                "<!ELEMENT foo ANY >" +
                "<!ENTITY xxe SYSTEM \"file:///C:/users/bdinger/documents/test.txt\" >]><foo>&xxe;</foo>";

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

        /// <summary>
        /// A4 - External XML Entities (XXE) - This will throw an exception when it parses the attempted attack 
        /// XML as the DtdProcessing has been set to Prohibit.
        /// </summary>
        /// <returns></returns>
        public ActionResult Feed()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"ISO - 8859 - 1\"?>" +
                         "<!DOCTYPE foo [" +
                         "<!ELEMENT foo ANY >" +
                         "<!ENTITY xxe SYSTEM \"file:///C:/users/bdinger/documents/test.txt\" >]><foo>&xxe;</foo>";

            XmlTextReader xmlReader = new XmlTextReader(new StringReader(xml));
            xmlReader.DtdProcessing = DtdProcessing.Prohibit;
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
