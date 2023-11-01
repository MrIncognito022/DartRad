using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DartRad.ViewModels
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required]
        [Display(Name ="Email Address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        
        public string Role { get; set; }

    }
}
