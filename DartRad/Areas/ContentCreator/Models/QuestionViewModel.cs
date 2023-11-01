using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.ContentCreator.Models
{
    public class QuestionViewModel
    {
        [Required]
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string AnswerExplanation { get; set; }
    }
    public class CreateQuestionViewModel : QuestionViewModel
    {
       
        public IFormFile HotspotImage { get; set; }
    }

    public class UpdateQuestionViewModel : CreateQuestionViewModel
    {
        [Required]
        public int Id { get; set; }
    }
    public class QuestionListViewModel : QuestionViewModel
    {
        public QuestionListViewModel()
        {
            AnswerMultipleChoices = new List<AnswerViewModel>();
        }
        public int Id { get; set; }
        public new string QuestionText { get; set; }
        public string HotspotQuestionImage { get; set; }
        public List<AnswerViewModel> AnswerMultipleChoices { get; set; }
        public List<AnswerViewModel> AnswerShortAnswer { get; set; }
        public List<AnswerMatchingViewModel> AnswerMatching { get; set; }
        public List<AnswerHotspotViewModel> AnswerHotspot { get; set; }
        public List<AnswerSequenceViewModel> AnswerSequence { get; set; }
    }
}
