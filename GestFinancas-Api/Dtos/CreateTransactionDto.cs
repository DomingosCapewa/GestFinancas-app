using System.ComponentModel.DataAnnotations;

namespace GestFinancas_Api.Dtos
{
    public class CreateTransactionDto
    {
        [Required]
        [StringLength(140, MinimumLength = 3)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 9999999999)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 2)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Income|Expense)$", ErrorMessage = "Type deve ser Income ou Expense")]
        public string Type { get; set; } = string.Empty;

        public DateTime? Date { get; set; }
    }
}
