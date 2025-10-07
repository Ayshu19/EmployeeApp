using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using EmployeeApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeApp.Data
{
    public class EmployeeRepository
    {
        private readonly IConfiguration _config;
        private string ConnectionString => _config.GetConnectionString("DefaultConnection");

        public EmployeeRepository(IConfiguration config)
        {
            _config = config;
        }

        // ---------------- Employee Methods ----------------

        public async Task<IEnumerable<EmployeeModel>> GetAllEmployeesAsync()
        {
            using var connection = new SqlConnection(ConnectionString);
            var sql = "SELECT * FROM Employees";
            return await connection.QueryAsync<EmployeeModel>(sql);
        }

        public async Task<EmployeeModel?> GetEmployeeByIdAsync(int id)
        {
            using var connection = new SqlConnection(ConnectionString);
            var sql = "SELECT * FROM Employees WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<EmployeeModel>(sql, new { Id = id });
        }

        public async Task AddEmployeeAsync(EmployeeModel employee)
        {
            using var connection = new SqlConnection(ConnectionString);
            var sql = @"
                INSERT INTO Employees 
                (Name, Department, Email, AadhaarNumber, PANNumber, DateOfBirth, Address)
                VALUES (@Name, @Department, @Email, @AadhaarNumber, @PANNumber, @DateOfBirth, @Address)";
            await connection.ExecuteAsync(sql, employee);
        }

        // ---------------- Attendance Methods ----------------

        public async Task<IEnumerable<AttendanceModel>> GetAttendanceByEmployeeAndMonthAsync(int employeeId, int year, int month)
        {
            using var connection = new SqlConnection(ConnectionString);
            var sql = @"
                SELECT * FROM Attendance
                WHERE EmployeeId = @EmployeeId
                  AND YEAR(Date) = @Year
                  AND MONTH(Date) = @Month";
            return await connection.QueryAsync<AttendanceModel>(sql, new { EmployeeId = employeeId, Year = year, Month = month });
        }

        public async Task AddAttendanceAsync(AttendanceModel attendance)
        {
            using var connection = new SqlConnection(ConnectionString);
            var sql = @"
                INSERT INTO Attendance
                (EmployeeId, Date, LeaveType)
                VALUES (@EmployeeId, @Date, @LeaveType)";
            await connection.ExecuteAsync(sql, attendance);
        }
    }
}
