namespace DartRad.ViewModels
{
    public class PublicQuizListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
        public int Questions { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
    }
}
