using UserRegFormHWv01.Services;

namespace UserRegFormHWv01.Middleware
{
    public class SessionAuthMiddleware
    {
        private readonly RequestDelegate next;

        public SessionAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService auth)
        {
            string UserId = context.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(UserId))
            {
                auth.SetUser(UserId);
                if (auth.User != null)
                {
                    auth.SetAuthInterval(Convert.ToInt64(context.Session.GetString("AuthMoment")));
                    if (auth.AuthInterval > 310)
                    {
                        context.Session.Remove("UserId");
                        context.Session.Remove("AuthMoment");
                        context.Response.Redirect("/");
                    }
                }
            }
            await next(context);
        }
    }
}
