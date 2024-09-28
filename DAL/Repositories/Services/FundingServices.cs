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
    public class FundingServices : IFundingServices
    {
        private readonly PeerlandingContext _peerLandingContext;
        private readonly IUserServices _userService;
        private readonly ILoanServices _loanService;
        private readonly IRepaymentServices _repaymentService;
        private readonly IMonthlyPaymentsServices _monthlyPaymentsService;

        public FundingServices(PeerlandingContext peerLandingContext, IUserServices userService, ILoanServices loanService, IRepaymentServices repaymentService, IMonthlyPaymentsServices monthlyPaymentsService)
        {
            _peerLandingContext = peerLandingContext;
            _userService = userService;
            _loanService = loanService;
            _repaymentService = repaymentService;
            _monthlyPaymentsService = monthlyPaymentsService;
        }

        public async Task<string> CreateFunding(ReqCreateFundingDto reqCreate)
        {
            var newFunding = new TrnFunding
            {
                Id = reqCreate.Id,
                LoanId = reqCreate.LoanId,
                LenderId = reqCreate.LenderId,
                Amount = reqCreate.Amount,
                FundedAt = reqCreate.FundedAt,
            };

            await _peerLandingContext.AddAsync(newFunding);
            await _peerLandingContext.SaveChangesAsync();

            return newFunding.Id;
        }

        public async Task<string> FundingLoan(ReqFundingLoanDto reqFunding)
        {
            var loan = await _loanService.GetLoanById(reqFunding.loanId) ?? throw new Exception("Loan not found");
            var lender = await _userService.GetUserById(reqFunding.lenderId) ?? throw new Exception("Lender not found");

            if (lender.Balance < loan.Amount)
            {
                throw new Exception("Insufficient lender balance");
            }

            using (var transaction = await _peerLandingContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _loanService.UpdateLoan(loan.LoanId, new ReqUpdateLoanDto
                    {
                        Status = "funded"
                    });

                    lender.Balance -= loan.Amount;
                    await _userService.Update(lender.Id, new ReqUpdateUserDto
                    {
                        Name = lender.Name,
                        Role = lender.Role,
                        Balance = lender.Balance
                    });

                    var user = await _userService.GetUserById(loan.User.Id) ?? throw new Exception("User not found");
                    user.Balance += loan.Amount;
                    await _userService.Update(user.Id, new ReqUpdateUserDto
                    {
                        Name = user.Name,
                        Role = user.Role,
                        Balance = user.Balance
                    });

                    await CreateFunding(new ReqCreateFundingDto
                    {
                        LenderId = lender.Id,
                        LoanId = loan.LoanId,
                        Amount = loan.Amount,
                        FundedAt = DateTime.UtcNow
                    });

                    var repayment = await _repaymentService.CreateRepayment(new ReqCreateRepaymentDto
                    {
                        LoanId = loan.LoanId,
                        Amount = loan.Amount,
                        RepaidAmount = 0,
                        BalanceAmount = loan.Amount,
                    });

                    await _monthlyPaymentsService.GenerateMonthlyPatments(repayment);

                    await transaction.CommitAsync();

                    return loan.LoanId;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Error occurred: {ex.Message}");
                }
            }
        }

        public async Task<ResFundingByIdDto> GetFundingByLoanId(string loanId)
        {
            var funding = await _peerLandingContext.TrnFundings
                .SingleOrDefaultAsync(l => l.LoanId == loanId);

            if (funding == null)
            {
                throw new Exception("funding not found");
            }

            var result = new ResFundingByIdDto
            {
                Id = funding.Id,
                Amount = funding.Amount,
                FundedAt = funding.FundedAt,
                LenderId = funding.LenderId,
                LoanId = funding.LoanId,
            };

            return result;
        }

        public async Task<List<ResFundingDto>> GetFundingsByLenderId(string lenderId)
        {
            var fundings = await _peerLandingContext.TrnFundings
                .Where(f => f.LenderId == lenderId)
                .ToListAsync();

            var fundingDtos = new List<ResFundingDto>();

            foreach (var funding in fundings)
            {
                var loan = await _loanService.GetLoanById(funding.LoanId);
                var borrower = await _userService.GetUserById(loan.User.Id);
                var repayment = await _repaymentService.GetRepaymentByLoanId(loan.LoanId);

                fundingDtos.Add(new ResFundingDto
                {
                    Id = funding.Id,
                    Borrower = new Borrower
                    {
                        Id = borrower.Id,
                        Name = borrower.Name
                    },
                    Amount = loan.Amount,
                    RepaidAmount = repayment.RepaidAmount,
                    RepaidStatus = repayment.RepaidStatus,
                    BalanceAmount = repayment.BalanceAmount,
                    PaidAt = repayment.PaidAt
                });
            }

            return fundingDtos;
        }
    }
}
