using EmployeeManagement.DataAccess;
using EmployeeManagement.Models;

namespace EmployeeManagement.Services
{
    public class EmployeeService
    {
        private readonly Repository _repository;

        public EmployeeService(Repository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            return await _repository.GetEmployeesAsync();
        }

        public async Task AddNewEmployeeAsync(EmployeeDto employee)
        {
            await _repository.AddEmployeeAsync(employee);
        }

    }
}
