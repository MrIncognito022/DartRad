using DartRad.Areas.ContentCreator.Models;

namespace DartRad.ViewModels
{
    public class PublicQuizViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ClinicalScenario { get; set; }

        public List<QuestionListViewModel> Questions { get; set; }
    }

    public class PublicQuestionViewModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public string AnswerExplanation { get; set; }
        public List<PublicAnswerMultipleChoiceViewModel> AnswerMultipleChoices { get; set; }
        public List<PublicMatchingAnswerViewModel> AnswerMatching { get; set; }
        public List<PublicAnsweShortAnswerViewModel> AnswerShortAnswer { get; set; }
        public List<AnswerHotspot> AnswerHotspot { get; set; }
        public List<PublicAnswerSequenceViewModel> AnswerSequence { get; set; }
        public string HotspotQuestionImageUrl { get; set; }
    }

    public class PublicAnswerMultipleChoiceViewModel
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class PublicAnsweShortAnswerViewModel
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
    }

    public class PublicMatchingAnswerViewModel
    {
        public int Id { get; set; }
        public string LeftSide { get; set; }
        public string RightSide { get; set; }
    }

    public class PublicAnswerSequenceViewModel
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public int Order { get; set; }
    }
   
}
