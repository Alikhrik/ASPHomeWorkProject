namespace UserRegFormHWv01.Middleware
{
    public static class SessionAsyncExtension
    {
        public static IApplicationBuilder
            UseSessionAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionAuthMiddleware>();
        }
    }
}
