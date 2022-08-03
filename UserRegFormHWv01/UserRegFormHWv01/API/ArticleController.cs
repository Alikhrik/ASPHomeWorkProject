﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserRegFormHWv01.DAL.Context;
using UserRegFormHWv01.Services;

namespace UserRegFormHWv01.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IntroContext _context;
        private readonly IHasher _hasher;

        public ArticleController(IntroContext context, IHasher hasher)
        {
            _context = context;
            _hasher = hasher;
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
            return _context.Articles.Where(a => a.TopicId == guid);
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

            var user = _context.Users.Find(userGuid);
            if (user == null)
                return new { status = "Error",  message = "Forbidden" };
            var topic = _context.Topics.Find(topicGuid);
            if (topic == null)
                return new { status = "Error", message = "Forbidden" };

            string HAvatarName = string.Empty;
            if (article.PictureFile != null)
                HAvatarName = AddAvatar(article.PictureFile);


            _context.Articles.Add(new() { 
                TopicId = topicGuid,
                Text = article.Text,
                AuthorId = userGuid,
                CreatedDate = DateTime.Now,
                PictureFile = HAvatarName
            });
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
    }
}
