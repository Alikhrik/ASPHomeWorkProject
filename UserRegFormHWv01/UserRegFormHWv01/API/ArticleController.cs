using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRegFormHWv01.DAL.Context;
using UserRegFormHWv01.Services;

namespace UserRegFormHWv01.API
{
    [Route("api/article")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IntroContext _context;
        private readonly IHasher _hasher;
        private readonly IAuthService _authService;

        public ArticleController(IntroContext context, IHasher hasher, IAuthService authService)
        {
            _context = context;
            _hasher = hasher;
            _authService = authService;
        }
        [HttpPut("{UserId}")]

        public IEnumerable<DAL.Entities.Article> Put(string UserId)
        {
            Guid guid;
            try { guid = Guid.Parse(UserId); }
            catch (Exception)
            {
                return null;
            }

            return _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Topic)
                .Include(a => a.Reply)
                .Where(a => a.AuthorId == guid && a.DeleteMoment == null)
                .OrderBy(a => a.CreatedDate);
        }

        [HttpGet("{TopicId}")]
        public IEnumerable<DAL.Entities.Article> Get(string TopicId)
        {
            Guid guid;
            try { guid = Guid.Parse(TopicId); }
            catch (Exception)
            {
                return null;
            }

            return _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Topic)
                .Include(a => a.Reply)
                .Where(a => a.TopicId == guid && a.DeleteMoment == null)
                .OrderBy(a => a.CreatedDate);
        }

        [HttpPost]

        public object Post([FromForm] Models.ArticleModel article)
        {
            string UserId = HttpContext.Request.Headers["User-Id"].ToString();
            Guid userGuid;
            try { userGuid = Guid.Parse(UserId); }
            catch (Exception) {
                return new
                {
                    status = "Error",
                    message = "User-Id Header empty or invalid (GUID expected)"
                };
            }

            if (string.IsNullOrEmpty(article.Text))
                return new { status = "Error", message = "Text empty" };

            Guid topicGuid;
            try { topicGuid = Guid.Parse(article.TopicId); }
            catch (Exception) {
                return new
                {
                    status = "Error",
                    message = "TopicId empty or invalid (GUID expected)"
                };
            }

            Guid repltIdGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(article.ReplyId))
            {
                try { repltIdGuid = Guid.Parse(article.ReplyId); }
                catch (Exception)
                {
                    return new
                    {
                        status = "Error",
                        message = "ReplyId invalid (GUID expected)"
                    };
                }
                var replyId = _context.Articles.Find(repltIdGuid);
                if (replyId == null)
                    return new { status = "Error", message = "Forbidden" };
            }

            var user = _context.Users.Find(userGuid);
            if (user == null)
                return new { status = "Error", message = "Forbidden" };
            var topic = _context.Topics.Find(topicGuid);
            if (topic == null)
                return new { status = "Error", message = "Forbidden" };


            string HAvatarName = string.Empty;
            if (article.PictureFile != null)
                HAvatarName = AddAvatar(article.PictureFile);

            var TimeNow = DateTime.Now;
            _context.Articles.Add(new() {
                TopicId = topicGuid,
                Text = article.Text,
                AuthorId = userGuid,
                CreatedDate = TimeNow,
                PictureFile = HAvatarName,
                ReplyId = (repltIdGuid == Guid.Empty ? null : repltIdGuid)
            });

            topic.LastArticleMoment = TimeNow;
            _context.SaveChanges();

            return new
            {
                status = "Ok",
                message = $"Article created"
            };
        }

        private string AddAvatar(IFormFile PictureFile)
        {
            string fullFileName = PictureFile.FileName;
            var fileType = fullFileName[fullFileName.LastIndexOf('.')..];
            var HashFileName = _hasher.Hash(fullFileName[..(fileType.Length - 2)] + DateTime.Now);
            string HFileName = HashFileName + fileType;

            using (var file = new FileStream("./wwwroot/img/" + HFileName, FileMode.Create))
                PictureFile.CopyTo(file);

            return HFileName;
        }

        [HttpDelete("{id}")]
        public object Delete(string id)
        {
            if (_authService.User == null)
            {
                return new { status = "Error", message = "Anauthorized" };
            }

            Guid articleId;
            try
            {
                articleId = Guid.Parse(id);
            }
            catch
            {
                return new { status = "Error", message = "Invalid id" };
            }

            var article = _context.Articles.Find(articleId);
            if (article == null)
            {
                return new { status = "Error", message = "Invalid article" };
            }

            if(_authService.User.Id != articleId)
            {
                return new { status = "Error", message = "Forbidden" };
            }

            return new { id };
        }
        public object Default(string? id)
        {
            return new { HttpContext.Request.Method, id };
        }
    }
}
