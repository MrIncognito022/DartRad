namespace DartRad.Areas.Editor.Models
{
    public class QuizNoteViewModel
    {
        public int Id { get; set; }
        public int AdminId { get; set; }

        public string AdminName { get; set; }
        public int QuizId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
