using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{
    public class Quiz : AuditableEntity
    {
        [Key]
        public int Id { get; set; }
        public int ContentCreatorId { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        public string ClinicalScenario { get; set; }
        public QuizStatus Status { get; set; }
        public int? ApprovedBy { get; set; }

        [ForeignKey("ApprovedBy")]
        public Editor ApprovedByAdmin { get; set; }

        [ForeignKey("ContentCreatorId")]
        public ContentCreator ContentCreator { get; set; }
        public List<QuizNote> Notes { get; set; }
        public string ImageUrl { get; set; }

        public string Category { get; set; }

    }
}
