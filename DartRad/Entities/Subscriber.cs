using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{
    public class Subscriber : AuditableEntity
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Institution { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public Editor CreatedByEditor { get; set; }
    }
}
