using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IRepaymentServices
    {
        public Task<string> CreateRepayment(ReqCreateRepaymentDto reqCreate);
        public Task<ResRepaymentDto> GetRepaymentByLoanId(string loanId);
        public Task<ResRepaymentDto> GetRepaymentById(string id);
    }
}
