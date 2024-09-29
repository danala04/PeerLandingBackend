using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BEPeer.Controllers
{
    [Route("rest/v1/monthlyPayment")]
    [ApiController]
    public class MonthlyPaymentController : Controller
    {
        private IMonthlyPaymentsServices _monthlyService;

        public MonthlyPaymentController(IMonthlyPaymentsServices monthlyService)
        {
            _monthlyService = monthlyService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMonthlyPaymentByRepaymentId(string repaymentId)
        {
            try
            {
                var response = await _monthlyService.GetMonthlyPaymentByRepaymentId(repaymentId);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success Payment"
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

        [HttpPost]
        [Route("pay")]
        [Authorize]
        public async Task<IActionResult> PayMonthlyPayments([FromBody] List<ReqPayMonthlyPaymentDto> paymentRequests)
        {
            try
            {
                var result = await _monthlyService.PayMonthlyPayments(paymentRequests);

                return Ok(new ResBaseDto<string>
                {
                    Data = result,
                    Success = true,
                    Message = "Payments updated successfully"
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
