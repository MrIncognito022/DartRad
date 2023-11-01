namespace DartRad.Entities
{
    public class AnswerSequence : AnswerBase
    {
        public string AnswerText { get; set; }
        // automatic management
        public int Order { get; set; }
    }
}
