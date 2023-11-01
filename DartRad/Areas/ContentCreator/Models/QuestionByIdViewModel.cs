using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.ContentCreator.Models
{
    public class QuestionByIdViewModel
    {
        public int Id { get; set; }
        [Required]
        public string QuestionText { get; set; }
        public QuestionType QuestionType { get; set; }
        public string AnswerExplanation { get; set; }
        public string ExistingImageName { get; set; }
        public string ExistingImagePath { get; set; }
    }
}
