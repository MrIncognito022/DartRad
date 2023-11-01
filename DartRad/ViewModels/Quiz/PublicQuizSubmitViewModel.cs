namespace DartRad.ViewModels
{
    public class PublicQuizSubmitViewModel
    {
        public int QuizId { get; set; }
        public List<PublicQuizSubmittedAnswerViewModel> Answers { get; set; }
       
    }

    public class PublicQuizSubmittedAnswerViewModel
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
        public string WrittenAnswer { get; set; }

        public List<PublicSubmittedMatchingAnswersViewModel> MatchedAnswers { get; set; }
    }

    public class PublicSubmittedMatchingAnswersViewModel
    {
        public int AnswerId { get; set; }
        public string Text { get; set; }
    }
}
