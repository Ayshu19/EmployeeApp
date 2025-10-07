using Microsoft.AspNetCore.Mvc;
using EmployeeApp.Data;
using EmployeeApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeApp.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly EmployeeRepository _repo;

        public AttendanceController(EmployeeRepository repo)
        {
            _repo = repo;
        }

        // ---------------- Display Attendance Page ----------------
        [HttpGet("/Employee/Attendance")]
        public IActionResult Index()
        {
            return View("~/Views/Employee/Attendance.cshtml");
        }

        // ---------------- Submit Leave ----------------
        [HttpPost("/Employee/Attendance")]
        public async Task<IActionResult> SubmitLeave(int employeeId, DateTime date, string leaveType)
        {
            if (employeeId <= 0)
            {
                ViewBag.Message = "Please enter a valid Employee ID.";
                return View("~/Views/Employee/Attendance.cshtml");
            }

            // Get current month's attendance BEFORE saving
            var attendances = await _repo.GetAttendanceByEmployeeAndMonthAsync(employeeId, date.Year, date.Month);

            int casualCount = attendances.Count(a => a.LeaveType == "Casual");
            int sickCount = attendances.Count(a => a.LeaveType == "Sick");

            string message;
            string finalType = leaveType;

            if (leaveType == "Casual" && casualCount >= 1)
            {
                finalType = "Loss of Pay";
                message = "Casual leave limit exceeded. Marked as Loss of Pay.";
            }
            else if (leaveType == "Sick" && sickCount >= 2)
            {
                finalType = "Loss of Pay";
                message = "Sick leave limit exceeded. Marked as Loss of Pay.";
            }
            else
            {
                message = "Leave successfully applied!";
            }

            // Save leave
            await _repo.AddAttendanceAsync(new AttendanceModel
            {
                EmployeeId = employeeId,
                Date = date,
                LeaveType = finalType
            });

            // Recalculate remaining leaves AFTER saving
            var updatedAttendances = await _repo.GetAttendanceByEmployeeAndMonthAsync(employeeId, date.Year, date.Month);
            int updatedCasual = updatedAttendances.Count(a => a.LeaveType == "Casual");
            int updatedSick = updatedAttendances.Count(a => a.LeaveType == "Sick");

            ViewBag.Message = message;
            ViewBag.LeavesLeft = $"Casual Left: {Math.Max(0, 1 - updatedCasual)}, Sick Left: {Math.Max(0, 2 - updatedSick)}";

            return View("~/Views/Employee/Attendance.cshtml");
        }

        // ---------------- Get Remaining Leaves via AJAX ----------------
        [HttpGet("/Employee/GetRemainingLeaves")]
        public async Task<JsonResult> GetRemainingLeaves(int employeeId, string leaveType, int year, int month)
        {
            var attendances = await _repo.GetAttendanceByEmployeeAndMonthAsync(employeeId, year, month);

            int casualCount = attendances.Count(a => a.LeaveType == "Casual");
            int sickCount = attendances.Count(a => a.LeaveType == "Sick");

            int remaining = leaveType == "Casual" ? Math.Max(0, 1 - casualCount)
                         : leaveType == "Sick" ? Math.Max(0, 2 - sickCount)
                         : 0;

            return Json(new { remaining });
        }
    }
}
