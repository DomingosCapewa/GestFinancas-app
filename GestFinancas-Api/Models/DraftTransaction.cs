public class DraftTransaction {
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public bool Confirmed { get; set; }
}

