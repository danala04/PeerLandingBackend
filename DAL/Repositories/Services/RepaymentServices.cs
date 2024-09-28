using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.Services
{
    public class RepaymentServices : IRepaymentServices
    {
        private readonly PeerlandingContext _peerLandingContext;

        public RepaymentServices(PeerlandingContext peerLandingContext)
        {
            _peerLandingContext = peerLandingContext;
        }

        public async Task<string> CreateRepayment(ReqCreateRepaymentDto reqCreate)
        {
            var newRepayment = new TrnRepayment
            {
                LoanId = reqCreate.LoanId,
                Amount = reqCreate.Amount,
                RepaidAmount = reqCreate.RepaidAmount,
                BalanceAmount = reqCreate.BalanceAmount,                
            };

            await _peerLandingContext.AddAsync(newRepayment);
            await _peerLandingContext.SaveChangesAsync();

            return newRepayment.Id;
        }

        public async Task<ResRepaymentDto> GetRepaymentById(string id)
        {
            var repayment = await _peerLandingContext.TrnRepayments
            .Where(r => r.Id == id)
            .Select(r => new ResRepaymentDto
            {
                Id = r.Id,
                Amount = r.Amount,
                BalanceAmount= r.BalanceAmount,
                LoanId= r.LoanId,
                PaidAt = r.PaidAt,
                RepaidAmount= r.RepaidAmount,
                RepaidStatus = r.RepaidStatus
                
            })
            .FirstOrDefaultAsync();

            if (repayment == null)
            {
                throw new Exception("User not found");
            }

            return repayment;
        }

        public async Task<ResRepaymentDto> GetRepaymentByLoanId(string loanId)
        {
            var repayment = await _peerLandingContext.TrnRepayments
                .Where(r => r.LoanId == loanId)
                .Select(r => new
                {
                    r.Id,
                    r.LoanId,
                    r.Amount,
                    r.RepaidAmount,
                    r.BalanceAmount,
                    r.RepaidStatus,
                    r.PaidAt
                })
                .FirstOrDefaultAsync();

            if (repayment == null)
            {
                throw new Exception("Repayment not found");
            }

            var result = new ResRepaymentDto
            {
                Id = repayment.Id,
                LoanId = repayment.LoanId,
                Amount = repayment.Amount,
                RepaidAmount = repayment.RepaidAmount,
                BalanceAmount = repayment.BalanceAmount,
                RepaidStatus = repayment.RepaidStatus,
                PaidAt = repayment.PaidAt,
            };

            return result;
        }

    }
}
