using System.ComponentModel.DataAnnotations;

public class DraftTransaction {
    public Guid Id { get; set; }

    public int UserId { get; set; }

    [Range(0.01, 9999999999)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(140, MinimumLength = 3)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(60, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;

    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public bool Confirmed { get; set; }
}

