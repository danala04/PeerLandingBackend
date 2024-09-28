using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IMonthlyPaymentsServices
    {
        Task<string> GenerateMonthlyPatments(string repaymentId);
        Task<List<ResMonthlyPaymentDto>> GetMonthlyPaymentByRepaymentId(string repaymentId);
        Task<string> PayMonthlyPayments(List<ReqPayMonthlyPaymentDto> payments);
    }
}
