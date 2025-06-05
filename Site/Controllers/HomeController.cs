using Microsoft.AspNetCore.Mvc;

namespace Site.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Route("/")]
    [Route("home")]
    public IActionResult Index()
    {
        return View();
    }

    [Route("about-us")]
    public IActionResult AboutUs()
    {
        return View();
    }
    
    [Route("contact-us")]
    public IActionResult ContactUs()
    {
        return View();
    }
    
    [Route("careers")]
    public IActionResult Careers()
    {
        return View();
    }
    
    [Route("products")]
    public IActionResult Products()
    {
        return View();
    }
    
}