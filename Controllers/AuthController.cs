using BoldReports.Processing.ObjectModels;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request)
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
                var token =  await _authService.Register(request);
                if (token == null)
                {
                    return Conflict(new ResponseModel
                    {
                        statusCode = 409,
                        message = "Email already exists",
                        data = "No data",
                        isSuccess = false
                    });
                }

                return Ok(new ResponseModel
                {
                    statusCode = 201,
                    message = "Registered successfully",
                    data = token,
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult>  Login(LoginRequest request)
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
                var token = await _authService.Login(request);
                if (token == null)
                {
                    return NotFound(new ResponseModel
                    {
                        statusCode = 404,
                        message = "User not found or invalid credentials",
                        data = "No data",
                        isSuccess = false
                    });
                }

                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "Login successful",
                    data = token,
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


      
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(forgotPasswordRequest request)
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
                var result = await _authService.Forgotpassword(request.email);
                if (!result)
                {
                    return NotFound(new ResponseModel
                    {
                        statusCode = 404,
                        message = "User not found or invalid email",
                        data = "No data",
                        isSuccess = false
                    });
                }

                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "Password reset email sent successfully",
                    data = "No data",
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

       
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPasswordWithOtp(ResetPasswordRequest request)
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
                var result =  await _authService.resetPassword(request);
                if (!result)
                {
                    return BadRequest(new ResponseModel
                    {
                        statusCode = 400,
                        message = "OTP do not match or unauthorized",
                        data = "No data",
                        isSuccess = false
                    });
                }

                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "Password has been reset",
                    data = "No data",
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

        [Authorize(Roles = "user,admin")]
        [HttpPost("upload-image")]
        public async Task<IActionResult> uploadProfile(IFormFile file)
        {
            try
            {

                var filepath = await _authService.uploadProfile(file);
                if (filepath == null)
                {
                    return BadRequest(new ResponseModel
                    {
                        statusCode = 400,
                        message = "Invalid file format or upload failed",
                        data = null,
                        isSuccess = false
                    });
                }


                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "File uploaded successfully",
                    data = filepath,
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

        [Authorize(Roles ="user,admin")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> updateProfile(userProfileRequest request)
        {
            try
            {
                var user=await _authService.changeProfile(request.userName, request.firstName, request.lastName);

                if (user == null)
                {
                    return BadRequest(new ResponseModel
                    {
                        statusCode = 400,
                        message = "no user is found",
                        data = null,
                        isSuccess = false
                    });
                }

                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "your changes are saved",
                    data = user,
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

        [HttpPut("change-Password")]
        [Authorize(Roles ="user,admin")]
        public async Task<IActionResult> changePassword(changePasswordRequest request)
        {
            try
            {
                var isChanged = await _authService.changePassword(request.OldPassword, request.NewPassword);
                if (!isChanged)
                {
                    return BadRequest(new ResponseModel
                    {
                        statusCode = 400,
                        message = "your old password is wrong",
                        data = "no data",
                        isSuccess = false
                    });
                }
                return Ok(new ResponseModel
                {
                    statusCode = 200,
                    message = "your password is changed",
                    data = "no data",
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
    }
}
