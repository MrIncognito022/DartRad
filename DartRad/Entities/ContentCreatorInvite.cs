using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DartRad.Entities
{
    public class ContentCreatorInvite
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string InvitationCode { get; set; }

        public int InvitedByAdminId { get; set; }
        [ForeignKey("InvitedByAdminId")]
        public Editor InvitedByAdmin { get; set; }

    }
}
