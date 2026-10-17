using MCStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MCStore.Domain.DTO
{
    public class CashInCashOutFeeDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int? WalletBalanceId { get; set; }
        public decimal StartAmount { get; set; }
        public decimal EndAmount { get; set; }
        public decimal Fee { get; set; }
        public string? WalletName { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public Operators Operator { get; set; }
    }

    public class CashInCashOutFeeCreateDto : IValidatableObject
    {
        [Required]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        public int? WalletBalanceId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal StartAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal EndAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }

        public Operators Operator { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartAmount > EndAmount)
            {
                yield return new ValidationResult("StartAmount must be less than or equal to EndAmount", new[] { nameof(StartAmount), nameof(EndAmount) });
            }
        }
    }

    public class CashInCashOutFeeUpdateDto : IValidatableObject
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ProductCode { get; set; }

        [Range(0, double.MaxValue)]
        public decimal StartAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal EndAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }

        public Operators Operator { get; set; }

        public int? walletBalanceId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartAmount > EndAmount)
            {
                yield return new ValidationResult("StartAmount must be less than or equal to EndAmount", new[] { nameof(StartAmount), nameof(EndAmount) });
            }
        }
    }
}
