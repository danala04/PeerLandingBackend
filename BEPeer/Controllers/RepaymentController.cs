using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BEPeer.Controllers
{
    [Route("rest/v1/repayment")]
    [ApiController]
    public class RepaymentController : Controller
    {
        private readonly IRepaymentServices _repaymentService;
        
        public RepaymentController(IRepaymentServices repaymentService)
        {
            _repaymentService = repaymentService;
        }

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetRepaymentByLoanId(string loanId)
        {
            try
            {
                var response = await _repaymentService.GetRepaymentByLoanId(loanId);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success Get Repayment"
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
