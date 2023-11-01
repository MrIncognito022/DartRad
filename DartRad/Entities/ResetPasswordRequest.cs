using System.ComponentModel.DataAnnotations;

namespace DartRad.Entities
{
    public class ResetPasswordRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime RequestTime { get; set; }

        public bool IsUsed { get; set; }
    }
}
