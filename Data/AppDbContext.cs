using Microsoft.EntityFrameworkCore;
using AccuStock.Models;

namespace AccuStock.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<BusinessYear> BusinessYears { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ChartOfAccountType> ChartOfAccountTypes { get; set; }
        public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding the Role data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "Operator" }
            );

            // Prevent multiple cascade paths
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Subscription)
                .WithMany()
                .HasForeignKey(b => b.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BusinessYear>()
            .HasOne(b => b.Subscription)
            .WithMany()
            .HasForeignKey(b => b.SubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChartOfAccount>().HasOne(ca => ca.Subscription).WithMany().HasForeignKey(ca=> ca.SubScriptionId).OnDelete(DeleteBehavior.Restrict);
        }

    }
}
