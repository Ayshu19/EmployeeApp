using Microsoft.AspNetCore.Mvc;
using EmployeeApp.Data;
using EmployeeApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

namespace EmployeeApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _repo;

        public EmployeeController(EmployeeRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new EmployeeModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeModel employee)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields.";
                return View(employee);
            }

            QuestPDF.Settings.License = LicenseType.Community;

            // ✅ Optional safety check for DateOfBirth
            if (employee.DateOfBirth.HasValue && employee.DateOfBirth.Value < new DateTime(1753, 1, 1))
                employee.DateOfBirth = null;

            // Save to DB
            await _repo.AddEmployeeAsync(employee);

            // Generate PDF
            var pdfBytes = GenerateEmployeePdf(employee);

            // Show message + download link + clear form
            ViewBag.Message = "Submitted successfully!";
            ViewBag.PdfFile = Convert.ToBase64String(pdfBytes);

            return View(new EmployeeModel());
        }

        private byte[] GenerateEmployeePdf(EmployeeModel employee)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Employee & KYC Details")
                            .FontSize(20)
                            .Bold()
                            .AlignCenter();

                        col.Item().Text("");

                        col.Item().Text($"Name: {employee.Name}");
                        col.Item().Text($"Department: {employee.Department}");
                        col.Item().Text($"Email: {employee.Email}");
                        col.Item().Text($"Aadhaar Number: {employee.AadhaarNumber}");
                        col.Item().Text($"PAN Number: {employee.PANNumber}");
                        col.Item().Text($"Date of Birth: {(employee.DateOfBirth.HasValue ? employee.DateOfBirth.Value.ToString("yyyy-MM-dd") : "")}");
                        col.Item().Text($"Address: {employee.Address}");
                    });
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}
