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
    [Authorize(Roles ="user,admin")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IProvablyFairService _provablyFairService;

        public GameController(IGameService gameService,IProvablyFairService provablyFairService)
        {
            _gameService = gameService;
            _provablyFairService = provablyFairService;
        }

        [HttpPost("startSession")]
        public IActionResult StartSession()
        {
            try
            {
                var session = _gameService.StartSession();
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "your session started",
                    data = session,
                    isSuccess = true
                });
            }catch (Exception ex)
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

        [HttpPost("stopSession")]
        public IActionResult StopSession()
        {
            try
            {
                 _gameService.EndSession();
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "your session stopped",
                    data = "no data",
                    isSuccess = true
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

        [HttpPost("performSpin")]
        public IActionResult Spin(SpinRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid data",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                var spinResult=_gameService.SpinReels(request.betAmount, request.clientSeed);
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = " your spin result",
                    data = spinResult,
                    isSuccess = true
                });
            }catch(Exception ex)
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
        [HttpPost("verifyOutcome")]
        public IActionResult verifyOutCome(verifyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    statusCode = 400,
                    message = "Invalid data",
                    data = "No data",
                    isSuccess = false
                });
            }
            try
            {
                var isValid=_provablyFairService.VerifyOutcome(request.serverSeed,request.clientSeed,request.nounce, request.providedHash);
                if(isValid)
                {
                    return Ok(new ResponseModel
                    {
                        statusCode = 200,
                        message = "no cheating",
                        data = "no data",
                        isSuccess = true
                    });
                }
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = " cheating",
                    data = "no data",
                    isSuccess = true
                });
            }
            catch(Exception ex)
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
