using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;
using Microsoft.AspNetCore.Mvc;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loan);
        Task<string> UpdateLoan(string loanId, ReqUpdateLoanDto reqUpdate);
        Task<ResListLoanDto> GetLoanById(string loanId);
        Task<List<ResListLoanDto>> GetAllLoans(string status);
        Task<List<ResListLoanDto>> GetAllLoansByUserId(string status, string userId);
    }
}
