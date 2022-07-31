using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using UserRegFormHWv01.DAL.Context;
using UserRegFormHWv01.Models;
using UserRegFormHWv01.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserRegFormHWv01.API
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IntroContext _context;
        private readonly IHasher _hasher;

        public UserController(IntroContext context, IHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        [HttpGet]
        public string Get(string? login, string? password)
        {
            if (string.IsNullOrEmpty(login))
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: login required";
            }
            if (string.IsNullOrEmpty(password))
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: password required";
            }

            DAL.Entities.User user =_context.Users.Where(u => u.Login == login).FirstOrDefault();
            if(user == null)
            {
                HttpContext.Response.StatusCode = 401;
                return "Unauthorized: credentials rejected";
            }

            string PassHash = _hasher.Hash(password, user.PassSalt);
            if(PassHash != user.PassHash)
            {
                HttpContext.Response.StatusCode = 401;
                return "Unauthorized: credentials invalid";
            }

            return user.Id.ToString();
        }

        // GET /api/user/bf5176c2-5fef-43b2-0d8b-08da6279ff46
        [HttpGet("{id}")]
        public object Get(string id)
        {
            Guid guid;
            try
            {
                guid = Guid.Parse(id);
            }
            catch
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: invalid id format (GUID required)";
            }
            return _context.Users.Find(guid);
            //return $"value {id}";
        }

        // POST api/<UserController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return $"POST {value} ";
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public object Put(string id, [FromBody] ChangeUserModel userData)
        {
            Guid guid;
            DAL.Entities.User FoundUser = null;
            try
            {
                guid = Guid.Parse(id);
                FoundUser = _context.Users.Find(guid);
                if (FoundUser == null) throw new Exception();
            }
            catch (FormatException)
            {
                HttpContext.Response.StatusCode = 409;
                return "Conflict: invalid id format (GUID required)";
            }
            catch (Exception)
            {
                HttpContext.Response.StatusCode = 404;
                return "Not found: user not found";
            }

            var changePass = ChangePasswordAsync(FoundUser, userData);
            var changeEmail = ChangeEmailAsync(FoundUser, userData);
            //user = new DAL.Entities.User
            //{
            //    Id = FoundUser.Id,
            //    RealName = userData.RealName ?? FoundUser.RealName,
            //    Login = userData.Login ?? FoundUser.Login,
            //    Email = userData.Email ?? FoundUser.Email,
            //    Avatar = userData.Avatar?.FileName ?? FoundUser.Avatar
            //};
            Task.WaitAll(new[] { changePass, changeEmail });
            _context.SaveChanges();

            return userData;
        }

        private void ChangePassword(DAL.Entities.User OldUser, ChangeUserModel NewUser)
        {
            if (OldUser == null || NewUser == null) return;
            if (string.IsNullOrEmpty(OldUser.PassHash) || string.IsNullOrEmpty(OldUser.PassSalt))
            {
                NewUser.NewPassword = null;
                NewUser.OldPassword = null;
                return;
            }

            if (string.IsNullOrEmpty(NewUser.NewPassword))
            {
                NewUser.NewPassword = "New password is empty";
                NewUser.OldPassword = null;
                return;
            }

            if (string.IsNullOrEmpty(NewUser.OldPassword))
            {
                NewUser.NewPassword = null;
                NewUser.OldPassword = "Old password is empty";
                return;
            }

            if (OldUser.PassHash != _hasher.UnHash(NewUser.OldPassword, OldUser.PassSalt))
            {
                NewUser.NewPassword = null;
                NewUser.OldPassword = "Old password incorrect";
                return;
            }

            if (NewUser.OldPassword == NewUser.NewPassword)
            {
                NewUser.NewPassword = "Old and new passwords must not match";
                NewUser.OldPassword = "Old and new passwords must not match";
                return;
            }

            var passHashSalt = _hasher.Hash(DateTime.Now.ToString());
            var passHash = _hasher.Hash(NewUser.NewPassword, passHashSalt);
            OldUser.PassHash = passHash;
            OldUser.PassSalt = passHashSalt;
            NewUser.NewPassword = "Changed";
            NewUser.OldPassword = "Changed";
        }
        private void ChangeEmail(DAL.Entities.User OldUser, ChangeUserModel NewUser)
        {
            if (OldUser == null || NewUser == null) return;
            if (string.IsNullOrEmpty(OldUser.Email) || string.IsNullOrEmpty(NewUser.NewEmail)) return;

            Regex regex = new(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)");

            if (!regex.IsMatch(NewUser.NewEmail))
            {
                NewUser.NewEmail = "incorrect";
                return;
            }
            OldUser.Email = NewUser.NewEmail;
            NewUser.NewEmail = "Changed";
        }
        private async Task ChangePasswordAsync(DAL.Entities.User OldUser, ChangeUserModel NewUser)
        {
            await Task.Run(() => ChangePassword(OldUser, NewUser));
        }
        private async Task ChangeEmailAsync(DAL.Entities.User OldUser, ChangeUserModel NewUser)
        {
            await Task.Run(() => ChangeEmail(OldUser, NewUser));
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return $"DELETE {id}";
        }
    }
}
