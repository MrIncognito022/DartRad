using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{
    public class QuizNote
    {
        [Key]
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int AdminId { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        [ForeignKey("AdminId")]
        public Editor Editor { get; set; }
    }
}
