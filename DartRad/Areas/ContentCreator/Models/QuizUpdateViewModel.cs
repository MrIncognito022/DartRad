namespace DartRad.Areas.ContentCreator.Models
{
    public class QuizUpdateViewModel : QuizCreateViewModel
    {
        public int Id { get; set; }
        public QuizStatus Status { get; set; }

        public string ImageUrl { get; set; }
    }
}
