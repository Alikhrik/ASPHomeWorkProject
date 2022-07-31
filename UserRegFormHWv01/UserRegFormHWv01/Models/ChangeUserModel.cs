namespace UserRegFormHWv01.Models
{
    public class ChangeUserModel
    {
        public string? NewRealName { get; set; }
        public string? NewLogin { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? NewEmail { get; set; }
        public IFormFile? NewAvatar { get; set; }
    }
}
