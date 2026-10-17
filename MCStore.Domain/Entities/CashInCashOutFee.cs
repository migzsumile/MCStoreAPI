using MCStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.Entities
{
    [Table("CashInCashOutFee", Schema = "Store")]
    public class CashInCashOutFee
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StartAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal EndAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; } = 0;

        //dropdown list of wallet balance in frontend
        public int? WalletBalanceId { get; set; }

        //dropdown list of operators in frontend
        public Operators Operator { get; set; } = Operators.add;

        public Product Product { get; set; }

        public WalletBalance WalletBalance { get; set; }
    }
}
