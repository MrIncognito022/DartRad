using System.ComponentModel.DataAnnotations;

namespace DartRad.Areas.SuperAdmin.Models
{
    public class AdminUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter an email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        //  ErrorMessage = "Password must be minimum 8 characters long and contain at least 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special character")]

        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
