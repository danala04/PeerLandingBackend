using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IFundingServices
    {
        Task<string> CreateFunding(ReqCreateFundingDto reqCreate);
        Task<string> FundingLoan(ReqFundingLoanDto reqFunding);
        Task<List<ResFundingDto>> GetFundingsByLenderId(string lenderId);
        Task<ResFundingByIdDto> GetFundingByLoanId(string loanId);
    }
}
