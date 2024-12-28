using EmployeeManagement.Models;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagement.DataAccess
{
    public interface IPaymentRepository
    {
        bool SavePayment(Payment payment);
        bool CreateOrder(OrderRequest orderRequest);
    }

    public class PaymentRepository : IPaymentRepository
    {
        // Simulate saving to the database
        public bool SavePayment(Payment payment)
        {
            // Here you would use Entity Framework or Dapper to save to a real database
            // For now, let's assume the save is always successful
            return true;
        }


        public bool CreateOrder(OrderRequest orderRequest)
        {

            return true;
        }
    }
}
