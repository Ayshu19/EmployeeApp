using Microsoft.AspNetCore.Mvc;

using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeApp.Models
{
    public class AttendanceModel
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(20)]
        public string LeaveType { get; set; } // "Casual", "Sick", "Loss of Pay"
    }
}

