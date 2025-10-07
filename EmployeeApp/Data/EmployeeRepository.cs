using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using EmployeeApp.Models;
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
    }
}
