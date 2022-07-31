using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Text.Json;
using UserRegFormHWv01.DAL.Context;
using UserRegFormHWv01.Models;
using UserRegFormHWv01.Services;

namespace UserRegFormHWv01.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IntroContext _introContext;
        private readonly IHasher _hasher;
        private readonly IAuthService _authService;

        public HomeController(ILogger<HomeController> logger, IntroContext introContext, IHasher hasher, IAuthService authService)
        {
            _logger = logger;
            _introContext = introContext;
            _hasher = hasher;
            _authService = authService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["AuthInterval"] = _authService.AuthInterval;
            ViewData["LoginUser"] = _authService.User;
            base.OnActionExecuting(context);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}