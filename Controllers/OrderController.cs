using EmployeeManagement.Models;
using EmployeeManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public OrderController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/order/submit
        [HttpPost("submit")]
        public IActionResult SubmitOrder([FromBody] OrderRequest orderRequest)
        {
            //Validate the request
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Call business layer to process the payment
            var result = _paymentService.ProcessOrder(orderRequest);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            // Return success response
            return Ok(new
            {
                Message = "orderRequest submitted successfully!",
                TransactionId = result.TransactionId,
                //UserName = orderRequest.UserName,
                //Amount = orderRequest.Amount
            });
        }
    }
}
