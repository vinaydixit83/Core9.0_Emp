using EmployeeManagement.Models;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagement.DataAccess
{
    public class Repository
    {
        private readonly string _connectionString;

        public Repository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<EmployeeDto>> GetEmployeesAsync()
        {
            var employees = new List<EmployeeDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetAllEmployees", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employees.Add(new EmployeeDto
                            {
                                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                Name = reader["Name"].ToString(),
                                Department = reader["Department"].ToString(),
                                Salary = Convert.ToDecimal(reader["Salary"])
                            });
                        }
                    }
                }
            }

            return employees;
        }


        public async Task AddEmployeeAsync(EmployeeDto employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("AddEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", employee.Name);
                    command.Parameters.AddWithValue("@Department", employee.Department);
                    command.Parameters.AddWithValue("@Salary", employee.Salary);
                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
