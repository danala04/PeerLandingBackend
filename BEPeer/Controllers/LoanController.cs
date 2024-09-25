using System.Text;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loan")]
    [ApiController]
    public class LoanController : Controller
    {
        private readonly ILoanServices _loanServices;

        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> AddLoan(ReqLoanDto reqLoan)
        {
            try
            {
                if (!ModelState.IsValid)
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

                var res = await _loanServices.CreateLoan(reqLoan);

                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Loan Added succesfully",
                    Data = res
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPut("{loanId}")]
        public async Task<IActionResult> UpdateLoan(string loanId, ReqUpdateLoanDto reqUpdate)
        {
            try
            {
                var response = await _loanServices.UpdateLoan(loanId, reqUpdate);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Loan Update Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLoan([FromQuery] string status = null)
        {
            try
            {
                var response = await _loanServices.GetAllLoans(status);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success Get All Loans"
                });
            }
            catch (Exception ex)
            {
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
