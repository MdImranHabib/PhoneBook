using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PBMS.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 11, ErrorMessage = "Invalid number")]
        public string Number { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }
    }
}
