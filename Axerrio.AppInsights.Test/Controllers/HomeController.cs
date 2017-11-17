using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Axerrio.AppInsights.Test.Models;
using Microsoft.Extensions.Logging;

namespace Axerrio.AppInsights.Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public IActionResult Index()
        {
            _logger.LogError(new ArgumentNullException(), "LogError First 2");
            //_logger.LogTrace("Trace Index browsed");
            _logger.LogDebug("Debug Index Browsed 2");
            _logger.LogInformation("Information Index Browsed 2");
            _logger.LogError(new ArgumentNullException(), "LogError Second 2");

            return View();
        }

        public IActionResult About()
        {
            _logger.LogTrace("About browsed");

            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            _logger.LogTrace("Contact browsed");

            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
