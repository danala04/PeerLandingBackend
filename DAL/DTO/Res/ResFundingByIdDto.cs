using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResFundingByIdDto
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string LenderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime FundedAt { get; set; }
    }
}
