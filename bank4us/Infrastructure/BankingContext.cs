using Bank4Us.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore;

namespace Bank4Us.Infrastructure
{
    public class BankingContext : DbContext
    {
        public BankingContext(DbContextOptions<BankingContext> options) : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transfer> Transfers => Set<Transfer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Configure primary keys and simple relationship
            modelBuilder.Entity<Account>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Transfer>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Account>()
            .OwnsOne(a => a.AccountHolder);

            // One Account can have many Transfers
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.Account)
                .WithMany() // No collection on Account, so WithMany() without navigation
                .HasForeignKey("AccountId") // shadow FK column
                .IsRequired(false);

            // Generate keys for Transfer.Id 
            modelBuilder.Entity<Transfer>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();

            // Generate keys for Account.Id 
            modelBuilder.Entity<Account>()
                .Property(t => t.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
