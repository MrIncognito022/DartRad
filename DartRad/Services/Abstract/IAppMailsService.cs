namespace DartRad.Services.Abstract
{
    public interface IAppMailsService
    {
        Task NotifyContentCreatorQuizStatus(bool isAccepted, int contentCreatorId, string quizName, string quizLink);
        Task NotifySubscribersOfNewQuiz( string quizUrl);
    }
}
