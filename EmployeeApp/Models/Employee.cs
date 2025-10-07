using Microsoft.AspNetCore.Mvc;

using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeApp.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string AadhaarNumber { get; set; }

        [Required]
        public string PANNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }  // ✅ Nullable

        [Required]
        public string Address { get; set; }
    }
}

