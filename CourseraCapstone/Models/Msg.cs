using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseraCapstone.Models
{
    public class Msg
    {
        public int Id { get; set; }
        [Required]
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        [Required]
        public string RecieverId { get; set; }
        public ApplicationUser Reciever { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
