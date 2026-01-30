public class Transaction {
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public TransactionSource Source { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}



public enum TransactionType
{
    Income,
    Expense
}
public enum TransactionSource
{
    Manual,
    AI
}