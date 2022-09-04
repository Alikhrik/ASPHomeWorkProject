using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UserRegFormHWv01.Services;

namespace UserRegFormHWv01.Controllers
{
    public class ForumController : Controller
    {
        public readonly IAuthService _authService;

        public ForumController(IAuthService authService)
        {
            _authService = authService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["AuthInterval"] = _authService.AuthInterval;
            ViewData["LoginUser"] = _authService.User;
            //base.OnActionExecuting(context);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Topic(string id)
        {
            ViewData["id"] = id;
            ViewData["authorId"] = _authService.User?.Id.ToString();
            return View("Topic");
        }
    }
}
