using Microsoft.EntityFrameworkCore;

namespace GestFinancas_Api.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; } = null!;
        public DbSet<Usuario> Usuario => Usuarios;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<DraftTransaction> DraftTransactions { get; set; } = null!;
        public DbSet<ConsentLog> ConsentLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        //    entity.Property(u => u.Token).HasMaxLength(256); 
        // entity.Property(u => u.TokenExpiracao).IsRequired(false); 
        }
    }
}
