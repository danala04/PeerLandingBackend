using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_repayments")]
    public class TrnRepayment
    {
        [Key]
        [Column("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey("Loan")]
        [Column("loan_id")]
        public string LoanId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("repaid_amount")]
        public decimal RepaidAmount { get; set; } = 0;

        [Required]
        [Column("balance_amount")]
        public decimal BalanceAmount {  get; set; }
        public MstLoans Loan { get; set; }

        [Required]
        [Column("repaid_status")]
        public string RepaidStatus { get; set; } = "on_repay";

        [Required]
        [Column("paid_at")]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    }
}
