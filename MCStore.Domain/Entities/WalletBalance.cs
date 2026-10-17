using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MCStore.Domain.Entities
{
    [Table("WalletBalance", Schema = "Store")]
    public class WalletBalance
    {
        public int Id { get; set; }

        public string WalletName { get; set; } = "";

        public decimal Balance { get; set; } = 0;

        public bool IsCurrent { get; set; }
    }
}
