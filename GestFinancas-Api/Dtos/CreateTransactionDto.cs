namespace GestFinancas_Api.Dtos
{
    public class CreateTransactionDto
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Type { get; set; } 
        public DateTime? Date { get; set; }
    }
}
