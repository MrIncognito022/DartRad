namespace DartRad.Extensions
{
    public static class EnumExtensions
    {
        public static string ToFriendlyString(this QuizStatus quizStatus)
        {
            if (quizStatus == QuizStatus.WaitingForApproval)
            {
                return "Waiting For Approval";
            }
            return quizStatus.ToString();
        }

        public static string ToFriendlyString(this QuestionType questionType)
        {
            return questionType.ToString().Replace("_", " ");
        }
    }
}
