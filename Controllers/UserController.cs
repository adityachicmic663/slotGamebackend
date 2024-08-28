using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlotGameBackend.Models;
using SlotGameBackend.Requests;
using SlotGameBackend.Services;

namespace SlotGameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="user")]
    public class UserController : ControllerBase
    {
        private readonly IWalletService _walletService;
        public UserController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("depositRequest")]
        public IActionResult CreateTransaction(DepositRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid amount",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                _walletService.CreateTransaction(request.amount, TransactionType.Deposit);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message="your request has been sent",
                    data="no data",
                    isSuccess=true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    statusCode = 500,
                    message = "Internal server error",
                    data = ex.InnerException?.Message ?? ex.Message,
                    isSuccess = false
                });
            }
        }
    }
}
