namespace DartRad.Areas.ContentCreator.Models
{
    public class QuizListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }

    }
}
