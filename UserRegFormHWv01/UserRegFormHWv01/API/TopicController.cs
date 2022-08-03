using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserRegFormHWv01.DAL.Context;
using UserRegFormHWv01.Services;

namespace UserRegFormHWv01.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly IntroContext _context;
        private readonly IHasher _hasher;

        public TopicController(IntroContext context, IHasher hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        // CREATE
        [HttpPost]

        public object Post(Models.TopicModel topic)
        {
            //HttpContext.Request.Headers["User-Id"].ToString();
            string UserId = HttpContext.Request.Headers["User-Id"].ToString();
            Guid guid;
            try { guid = Guid.Parse(UserId); }
            catch (Exception)
            {
                return new
                {
                    status = "Error",
                    message = "User-Id Header empty or invalid (GUID expected)"
                };
            }
            string Culture = HttpContext.Request.Headers["Culture"].ToString().ToLower();
            string[] SupportedCultures = new string[] { "uk-ua", "en-gb" };
            if (Array.IndexOf(SupportedCultures, Culture) == -1)
            {
                return new
                {
                    status = "Error",
                    message = "Culture Header empty or invalid or not supported"
                };
            }
            HttpContext.Response.Headers.Add("Culture", Culture);

            if (topic == null)
                return new { status = "Error", message = "No data" };

            if (string.IsNullOrEmpty(topic.Title)
                || string.IsNullOrEmpty(topic.Description))
                return new { status = "Error", message = "Empty title or description" };

            var user = _context.Users.Find(guid);
            if (user == null)
            {
                return new
                {
                    status = "Error",
                    message = "Forbidden"
                };
            }

            if (_context.Topics.Where(t => t.Title == topic.Title).Any())
            {
                return new
                {
                    status = "Error",
                    message = $"Topic '{topic.Title}' title does exist"
                };
            }

            _context.Topics.Add(new()
            {
                Title = topic.Title,
                Description = topic.Description,
                AuthorId = user.Id
            });
            _context.SaveChangesAsync();

            return new
            {
                status = "Ok",
                message = $"Topic '{topic.Title}' created"
            };
        }

        [HttpGet]
        public IEnumerable<DAL.Entities.Topic> Get()
        {
            return _context.Topics;
        }
    }
}
