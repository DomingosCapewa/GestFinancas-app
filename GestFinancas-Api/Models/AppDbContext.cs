using Microsoft.EntityFrameworkCore;

namespace GestFinancas_Api.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuario { get; set; } = null!;
        public DbSet<Transaction> Transaction { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<DraftTransaction> DraftTransaction { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        //    entity.Property(u => u.Token).HasMaxLength(256); 
        // entity.Property(u => u.TokenExpiracao).IsRequired(false); 
        }
    }
}
