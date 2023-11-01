using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{
    public class Question : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; }
        public string AnswerExplanation { get; set; }
        public QuestionType QuestionType { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        public List<AnswerMultipleChoice> AnswerMultipleChoices { get; set; }
        public List<AnswerShortAnswer> AnswerShortAnswer { get; set; }
        public List<AnswerMatching> AnswerMatching { get; set; }
        public List<AnswerHotspot> AnswerHotspot { get; set; }
        public List<AnswerSequence> AnswerSequence { get; set; }
        public virtual HotspotQuestionImage HotspotQuestionImage { get; set; }
    }
}
