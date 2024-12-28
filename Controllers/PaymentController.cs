using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/payment/submit
        [HttpPost("submit")]
        public IActionResult SubmitPayment([FromBody] PaymentRequest paymentRequest)
        {
            //Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Call business layer to process the payment
            var result = _paymentService.ProcessPayment(paymentRequest);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            // Return success response
            return Ok(new
            {
                Message = "Payment submitted successfully!",
                TransactionId = result.TransactionId,
                UserName = paymentRequest.UserName,
                Amount = paymentRequest.Amount
            });
        }
    }
}
