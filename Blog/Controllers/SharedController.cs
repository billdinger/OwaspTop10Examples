using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Blog.Controllers
{
    public class SharedController : Controller
    {

        [Route("/Error")]
        public IActionResult Index()
        {
            return View("Error");
        }

    }
}