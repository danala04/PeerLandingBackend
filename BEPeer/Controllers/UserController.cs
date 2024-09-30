using System.Text;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BEPeer.Controllers
{
    [Route("rest/v1/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(ReqRegisterUserDto register)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList(),
                        }).ToList();

                    var errorMessage = new StringBuilder("Validation errors occured!");

                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }

                var res = await _userServices.Register(register);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "User registered succesfully",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                if(ex.Message == "Email already used")
                {
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin,lender")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var users = await _userServices.GetAllUser();
                return Ok(new ResBaseDto<List<ResUserDto>>
                {
                    Success = true,
                    Message = "List of users",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResUserDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromQuery] string userId = null)
        {
            try
            {
                var users = await _userServices.GetUserById(userId);
                return Ok(new ResBaseDto<ResUserByIdDto>
                {
                    Success = true,
                    Message = "Get User Success!",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<ResUserByIdDto>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto loginDto)
        {
            try
            {
                var response = await _userServices.Login(loginDto);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "User Login Success"
                });
            }
            catch(Exception ex)
            {
                if(ex.Message == "Invalid email or password")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResLoginDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "admin,lender")]
        public async Task<IActionResult> Update(string userId, ReqUpdateUserDto reqUpdate)
        {
            try
            {
                var response = await _userServices.Update(userId, reqUpdate);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "User Update Success"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResLoginDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var result = await _userServices.Delete(userId);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User deleted successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
