using UserRegFormHWv01.DAL.Entities;

namespace UserRegFormHWv01.Services
{
    public interface IAuthService
    {
        public User User { get; set; }

        public long AuthInterval { get; set; }

        public void SetUser(string id);
        public void SetAuthInterval(long authMoment);
    }
}
