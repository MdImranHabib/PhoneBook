using Microsoft.AspNetCore.Mvc;
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
       
        [StringLength(100, ErrorMessage ="Name can't be greater than 50 characters!")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(200, ErrorMessage = "Address can't be Greater than 100 Characters")]
        public string Address { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email")]       
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        [StringLength(15, MinimumLength = 11, ErrorMessage = "Invalid number")]
        [Remote(action: "IsNumberExist", controller: "Contacts", ErrorMessage = "This number already exist. Try another")]
        public string Number { get; set; }

        [Required]
        public int GroupId { get; set; }

        public virtual Group Group { get; set; }
    }
}
