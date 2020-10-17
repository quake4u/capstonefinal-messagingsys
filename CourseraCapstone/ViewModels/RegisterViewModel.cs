using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseraCapstone.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [Remote("isUsernameInUse", "account")]
        [RegularExpression(@"[A-Za-z][A-Za-z0-9.]{6,30}", ErrorMessage = "Invalid Username (Username must be between 6 and 30 characters)")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        [Remote("isEmailInUse", "account")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and Confirm password do not match")]
        public string ConfirmPassword { get; set; }
    }
}