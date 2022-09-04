using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using System.Text.RegularExpressions;
using UserRegFormHWv01.DAL.Context;
using UserRegFormHWv01.DAL.Entities;
using UserRegFormHWv01.Models;
using UserRegFormHWv01.Services;

namespace UserRegFormHWv01.Controllers
{
    public class AuthController : Controller
    {
        private readonly IntroContext _introContext;
        private readonly IHasher _hasher;
        private readonly IAuthService _authService;
        public AuthController(IntroContext introContext, IHasher hasher, IAuthService authService)
        {
            _hasher = hasher;
            _introContext = introContext;
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
            string AuthError = HttpContext.Session.GetString("AuthError");
            if (!string.IsNullOrEmpty(AuthError))
            {
                ViewData["AuthError"] = AuthError;
                HttpContext.Session.Remove("AuthError");
            }
            return View();
        }
        public IActionResult Register()
        {
            string strUser = HttpContext.Session.GetString("RegUser");
            string strErrors = HttpContext.Session.GetString("Errors");
            if (!string.IsNullOrEmpty(strUser) && !string.IsNullOrEmpty(strErrors))
            {
                ViewData["RegUser"] = JsonSerializer.Deserialize<RegUserModel>(strUser);
                ViewData["err"] = JsonSerializer.Deserialize<string[]>(strErrors);
            }
            return View();
        }

        [HttpPost]
        public RedirectResult RegUser(RegUserModel regUser)
        {
            string[] err = new string[8];
            if (regUser == null)
            {
                err[0] = "Некорректный вызов (нет данных)";
            }
            else
            {
                if (String.IsNullOrEmpty(regUser.RealName))
                    err[1] = "RealName не может быть пустым";

                if (String.IsNullOrEmpty(regUser.Login))
                    err[2] = "Login не может быть пустым";

                if (String.IsNullOrEmpty(regUser.Password1))
                    err[3] = "Password1 не может быть пустым";

                if (String.IsNullOrEmpty(regUser.Password2))
                    err[4] = "Password2 не может быть пустым";

                if (String.IsNullOrEmpty(regUser.Email))
                    err[5] = "Email не может быть пустым";

                if (String.IsNullOrEmpty(err[3]) && String.IsNullOrEmpty(err[4]) && regUser.Password1 != regUser.Password2)
                    err[6] = "Password2 не соответствует Password1";

                var user = _introContext.Users.Where(u => u.Login == regUser.Login);
                if (user.Count() != 0)
                    err[7] = "Введеный Login уже есть в базе";
            }

            HttpContext.Session.SetString("Errors", JsonSerializer.Serialize(err));

            bool IsValid = true;

            foreach (string error in err)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    IsValid = false;
                    break;
                }
            }

            if (IsValid)
            {
                var passHashSalt = _hasher.Hash(DateTime.Now.ToString());
                var passHash = _hasher.Hash(regUser.Password1, passHashSalt);

                string HFileName = "no-avatar.png";

                if (regUser.Avatar != null)
                {
                    string fullFileName = regUser.Avatar.FileName;
                    var fileType = fullFileName[fullFileName.LastIndexOf('.')..];
                    var HashFileName = _hasher.Hash(fullFileName[..(fileType.Length - 2)] + DateTime.Now);
                    HFileName = HashFileName + fileType;
                    regUser.Avatar.CopyTo(new FileStream(".wwwroot/img/" + HFileName, FileMode.Create));
                }

                _introContext.Users.Add(new User
                {
                    RealName = regUser.RealName,
                    Login = regUser.Login,
                    PassHash = passHash,
                    PassSalt = passHashSalt,
                    Email = regUser.Email,
                    Avatar = HFileName
                });
                _introContext.SaveChanges();
                return Redirect("/");
            }
            else
            {
                regUser.Password1 = String.Empty;
                regUser.Password2 = String.Empty;
                regUser.Avatar = null;
                HttpContext.Session.SetString("RegUser", JsonSerializer.Serialize(regUser));
            }


            return Redirect("/Auth/Register");
        }

        [HttpPost]
        public RedirectResult Login(string UserLogin, string UserPassword)
        {
            User user;
            try
            {
                if (string.IsNullOrEmpty(UserLogin))
                    throw new Exception("Login Empty");

                if (string.IsNullOrEmpty(UserPassword))
                    throw new Exception("Password Empty");

                user = _introContext.Users.Where(u => u.Login == UserLogin).FirstOrDefault();

                if (user == null)
                    throw new Exception("Login invalid");

                string PassHash = _hasher.Hash(UserPassword, user.PassSalt);

                if (PassHash != user.PassHash)
                    throw new Exception("Password invalid");
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("AuthError", ex.Message);
                return Redirect("/Auth/Index");
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());

            HttpContext.Session.SetString("AuthMoment", DateTime.Now.Ticks.ToString());

            return Redirect("/");
        }

        public IActionResult Exit()
        {

            string UserId = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(UserId) != null)
            {
                HttpContext.Session.Remove("UserId");
            }

            return Redirect("/Auth/Index");
        }

        public IActionResult Profile()
        {
            return View();
        }

        public string ChangeRealName(string NewName)
        {
            if (_authService.User == null)
                return "Forbidden";

            Regex regex = new(@"^([A-Z]|[a-z])+$");

            if (!regex.IsMatch(NewName))
                return "incorrect";

            return string.Empty;
        }
        public string ChangeEmail(string NewEmail)
        {
            if (_authService.User == null)
                return "Forbidden";

            Regex regex = new(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)");

            if (!regex.IsMatch(NewEmail))
                return "incorrect";

            return string.Empty;
        }

        [HttpPost]

        public JsonResult ChangeLogin([FromBody] string NewLogin)
        {
            try
            {
                if (string.IsNullOrEmpty(NewLogin))
                    throw new Exception("Login could not be empty");
                if (!Regex.IsMatch(NewLogin, @"^([A-Z]|[a-z]|[0-9]){5}([A-Z]|[a-z]|[0-9])*$"))
                    throw new Exception("Login incorrect");
                if (_introContext.Users.Where(l => l.Login == NewLogin).Count() > 0)
                    throw new Exception("Login in use");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            _authService.User.Login = NewLogin;
            _introContext.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult ChangePassword([FromBody] ChangePasswordModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.NewPassword))
                    throw new Exception("New password is empty");

                if (string.IsNullOrEmpty(model.OldPassword))
                    throw new Exception("Old password is empty");

                if (_authService.User.PassHash != _hasher.UnHash(model.OldPassword, _authService.User.PassSalt))
                    throw new Exception("Old password incorrect");

                if (model.OldPassword == model.NewPassword)
                    throw new Exception("Old and new passwords must not match");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            var passHashSalt = _hasher.Hash(DateTime.Now.ToString());
            var passHash = _hasher.Hash(model.NewPassword, passHashSalt);
            _authService.User.PassHash = passHash;
            _authService.User.PassSalt = passHashSalt;
            _introContext.SaveChanges();
            return Json(string.Empty);
        }

        [HttpPost]

        public JsonResult ChangeAvatar(IFormFile userAvatar)
        {
            try
            {
                if (userAvatar == null)
                    throw new Exception("Avatar is null");
            }
            catch (Exception ex)
            {
                return Json(new {Status = "Error", Message = ex });
            }

            string fullFileName = userAvatar.FileName;
            var fileType = fullFileName[fullFileName.LastIndexOf('.')..];
            var HashFileName = _hasher.Hash(fullFileName[..(fileType.Length - 2)] + DateTime.Now);
            string HFileName = HashFileName + fileType;

            System.IO.File.Delete("./wwwroot/img/" + _authService.User.Avatar);
            using (var file = new FileStream("./wwwroot/img/" + HFileName, FileMode.Create))
            userAvatar.CopyTo(file);

            _authService.User.Avatar = HFileName;
            _introContext.SaveChanges();

            JsonResult a = Json(new { Status = "Ok", Message = HFileName });
            return a;
        }
    }
}
