using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class TrnFunding
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loan")]
        [Column("loan_id")]
        public string LoanId { get; set; }

        [Required]
        [ForeignKey("Lender")]
        [Column("lender_id")]
        public string LenderId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("funded_at")]
        public DateTime FundedAt { get; set; }

        public MstLoans Loan { get; set; }
        public MstUser Lender { get; set; }
    }
}
