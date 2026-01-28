public class AuditLog {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; }
    public string Entity { get; set; }
    public Guid EntityId { get; set; }
    public DateTime Timestamp { get; set; }
}
