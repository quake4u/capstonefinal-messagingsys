using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseraCapstone.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        [Display(Name = "To (Username)")]
        [Remote("IsUsernameExists", "Account")]
        [RegularExpression(@"[A-Za-z][A-Za-z0-9.]{6,30}", ErrorMessage = "Invalid Username")]
        public string UserName { get; set; }
        [Required]
        [MaxLength(500, ErrorMessage = "Message can not exceed 500 characters")]
        public string Content { get; set; }
    }
}
