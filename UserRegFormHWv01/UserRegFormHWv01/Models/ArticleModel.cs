namespace UserRegFormHWv01.Models
{
    public class ArticleModel
    {
        public string TopicId { get; set; }
        public string Text { get; set; }
        public IFormFile? PictureFile { get; set; }
    }
}
