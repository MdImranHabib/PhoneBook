using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PBMS.Models
{
    public class Contact
    {
        public int Id { get; set; }
       
        [StringLength(50, ErrorMessage ="Name can't be greater than 50 characters!")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(100, ErrorMessage = "Address can't be Greater than 100 Characters")]
        public string Address { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        [StringLength(20, ErrorMessage = "Contact number can't be Greater than 20 Characters")]
        public string Number { get; set; }

        [StringLength(50, ErrorMessage = "Occupation can't be Greater than 50 Characters")]
        public string Occupation { get; set; }
    }
}
