using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.Editor.Models
{
    public class ContentCreatorInviteViewModel
    {
        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}
