using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{
    public class HotspotQuestionImage
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question Question { get; set; }
        public string ImageUrl { get; set; }
    }
}
