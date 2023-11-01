﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DartRad.Areas.Editor.Models
{
    public class ContentCreatorUpdateViewModel
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

        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
