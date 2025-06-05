using Microsoft.AspNetCore.Mvc;

namespace Site.Controllers
{
    [Route("admin")]
    [Route("{url}/admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}