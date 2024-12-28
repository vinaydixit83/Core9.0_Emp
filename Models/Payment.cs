using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class PaymentRequest
    {
        public string EventName { get; set; }
        public string RequestId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // e.g., CreditCard, PayPal, etc.

        [StringLength(500)]
        public string Notes { get; set; }
    }

    public class Payment
    {
        public string UserName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        public string TransactionId { get; set; }
    }

    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }





    public class OrderResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }


    public class OrderRequest
    {
        public string? orderId;
        public string? requestId;
        public string? customerId;
        public string?   eventName;
        public Items[] ItemsList;
        public int totalAmount;
        public string? timeStamp;

        public bool IsSuccess { get; internal set; }
    }

    public class Items
    {
        public string? itemID;
        public int quantity;
    }

}
