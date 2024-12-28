using EmployeeManagement.Models;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagement.DataAccess
{
    public class RabbitMsgRepository
    {
        private readonly string _connectionString;

        public RabbitMsgRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        //public async Task<List<EmployeeDto>> GetEmployeesAsync()
        //{
        //    var employees = new List<EmployeeDto>();

        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        using (var command = new SqlCommand("GetAllEmployees", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;

        //            connection.Open();
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                while (await reader.ReadAsync())
        //                {
        //                    employees.Add(new EmployeeDto
        //                    {
        //                        EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
        //                        Name = reader["Name"].ToString(),
        //                        Department = reader["Department"].ToString(),
        //                        Salary = Convert.ToDecimal(reader["Salary"])
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return employees;
        //}


        //public async Task AddMessagesToQueueAsync(QueueMessage queueMessage)
        //{
        //    using (var connection = new SqlConnection("data source=VINAY\\SQLEXPRESS;database=EmployeeManagementPortal; Persist Security Info=True;Integrated Security=true; Application Name=EmployeePortal"))
        //    {
        //        using (var command = new SqlCommand("AddMessageToMq", connection))
        //        {
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@message", queueMessage.message);
        //            command.Parameters.AddWithValue("@queuename", queueMessage.queuename);
        //            command.Parameters.AddWithValue("@exchangename", queueMessage.exchangename);
        //            command.Parameters.AddWithValue("@routingkey", queueMessage.routingkey);
        //            connection.Open();
        //            await command.ExecuteNonQueryAsync();
        //        }
        //    }
        //}
    }
}
