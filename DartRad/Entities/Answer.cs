using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{
    public abstract class AnswerBase : AuditableEntity
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
    }
}
