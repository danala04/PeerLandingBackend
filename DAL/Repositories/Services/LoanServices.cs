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
    public class LoanServices : ILoanServices
    {
        private readonly PeerlandingContext _peerLandingContext;

        public LoanServices(PeerlandingContext peerLandingContext)
        {
            _peerLandingContext = peerLandingContext;
        }

        public async Task<string> CreateLoan(ReqLoanDto loan)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loan.BorrowerId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.Duration,
            };

            await _peerLandingContext.AddAsync(newLoan);
            await _peerLandingContext.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        public async Task<List<ResListLoanDto>> GetAllLoans(string status)
        {
            var loansQuery = _peerLandingContext.MstLoans
                .Include(l => l.User)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    User = new User
                    {
                        Id = loan.User.Id,
                        Name = loan.User.Name
                    },
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                });

            if (!string.IsNullOrEmpty(status))
            {
                loansQuery = loansQuery.Where(loan => loan.Status == status);
            }

            loansQuery = loansQuery.OrderBy(loan => loan.CreatedAt);

            Console.WriteLine(loansQuery);

            return await loansQuery.ToListAsync();
        }

        public async Task<List<ResListLoanDto>> GetAllLoansByUserId(string status, string userId)
        {
            var loansQuery = _peerLandingContext.MstLoans
                .Include(l => l.User)
                .Select(loan => new ResListLoanDto
                {
                    LoanId = loan.Id,
                    User = new User
                    {
                        Id = loan.User.Id,
                        Name = loan.User.Name
                    },
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.Duration,
                    Status = loan.Status,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                })
                .Where(loan => loan.User.Id == userId);

            if (!string.IsNullOrEmpty(status))
            {
                loansQuery = loansQuery.Where(loan => loan.Status == status);
            }

            loansQuery = loansQuery.OrderBy(loan => loan.CreatedAt);

            return await loansQuery.ToListAsync();
        }

        public async Task<string> UpdateLoan(string loanId, ReqUpdateLoanDto reqpUpdate)
        {
            var existingLoan = await _peerLandingContext.MstLoans.SingleOrDefaultAsync(loan => loan.Id == loanId);

            if (existingLoan == null)
            {
                throw new Exception("loan not found");
            }

            existingLoan.Status = reqpUpdate.Status;
            existingLoan.UpdatedAt = DateTime.UtcNow;

            _peerLandingContext.MstLoans.Update(existingLoan);
            await _peerLandingContext.SaveChangesAsync();

            return existingLoan.BorrowerId;
        }
    }
}
