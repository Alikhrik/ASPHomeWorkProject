using UserRegFormHWv01.DAL.Context;
using UserRegFormHWv01.DAL.Entities;

namespace UserRegFormHWv01.Services
{
    public class SessionAuthServece : IAuthService
    {
        private readonly IntroContext _context;
        public SessionAuthServece(IntroContext context)
        {
            _context = context;
        }
        public User User { get; set; }

        public long AuthInterval { get; set; }

        public void SetUser(string id)
        {
            User = _context.Users.Find(Guid.Parse(id));
        }

        public void SetAuthInterval(long authMoment)
        {
            AuthInterval = (DateTime.Now.Ticks - authMoment) / (long)1e7;
        }
    }
}
