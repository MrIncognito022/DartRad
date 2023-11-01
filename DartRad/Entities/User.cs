using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{

    public abstract class BaseUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
    public class SuperAdmin : BaseUser
    {
        public DateTime CreatedAt { get; set; }
    }
    public class Editor : BaseUser
    {
        public int CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public SuperAdmin CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ContentCreator : BaseUser
    {
        public int InvitedBy { get; set; }
        [ForeignKey("InvitedBy")]
        public Editor InvitedByUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Quiz> Quizzes { get; set; }
    }
}
