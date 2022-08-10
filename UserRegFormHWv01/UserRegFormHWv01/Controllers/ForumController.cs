using Microsoft.AspNetCore.Mvc;

namespace UserRegFormHWv01.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
