using System.ComponentModel.DataAnnotations;

namespace DartRad.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
