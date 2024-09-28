using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.DTO.Res
{
    public class ResRepaymentDto
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string LoanId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public decimal RepaidAmount { get; set; }

        [Required]
        public decimal BalanceAmount { get; set; }

        [Required]
        public string RepaidStatus { get; set; }

        [Required]
        public DateTime PaidAt { get; set; }
    }

}
