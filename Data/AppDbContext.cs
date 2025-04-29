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
        public DbSet<OpeningBalances> OpeningBalances { get; set; }
        public DbSet<JournalPost> JournalPosts { get; set; }
        public DbSet<JournalPostDetail> JournalPostDetails { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email)
                    .IsUnique();
                    
            // Seeding the Role data
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin" },
                new Role { Id = 2, Name = "Admin" },
                new Role { Id = 3, Name = "Operator" }
            );         

            // Seeding the Units

            modelBuilder.Entity<Unit>().HasData(

                new Unit { Id = 1, Name = "Piece" },
                new Unit { Id = 2, Name = "Box" },
                new Unit { Id = 3, Name = "Gram " },
                new Unit { Id = 4, Name = "Liter" },
                new Unit { Id = 5, Name = "Meter " },
                new Unit { Id = 6, Name = "Foot " },
                new Unit { Id = 7, Name = "Inch " },
                new Unit { Id = 8, Name = "Unit" },
                new Unit { Id = 9, Name = "Kg" }
                );

            modelBuilder.Entity<ChartOfAccountType>().HasData(
    // Top-Level Account Types (ParentId = 0)
    new ChartOfAccountType { Id = 1, Name = "Assets", ParentId = 0, GroupID = 1 },
    new ChartOfAccountType { Id = 2, Name = "Liabilities", ParentId = 0, GroupID = 1 },
    new ChartOfAccountType { Id = 3, Name = "Equity", ParentId = 0, GroupID = 1 },
    new ChartOfAccountType { Id = 4, Name = "Income", ParentId = 0, GroupID = 1 },
    new ChartOfAccountType { Id = 5, Name = "Expense", ParentId = 0, GroupID = 1 },

    // Child Accounts under "Assets" (ParentId = 1)
    new ChartOfAccountType { Id = 6, Name = "Other Assets", ParentId = 1, GroupID = 0 },
    new ChartOfAccountType { Id = 7, Name = "Other Current Assets", ParentId = 1, GroupID = 0 },
    new ChartOfAccountType { Id = 8, Name = "Cash", ParentId = 1, GroupID = 0 },
    new ChartOfAccountType { Id = 9, Name = "Bank", ParentId = 1, GroupID = 0 },
    new ChartOfAccountType { Id = 10, Name = "Fixed Assets", ParentId = 1, GroupID = 0 },
    new ChartOfAccountType { Id = 11, Name = "Stock", ParentId = 1, GroupID = 0 },
    new ChartOfAccountType { Id = 12, Name = "Payment Clearing", ParentId = 1, GroupID = 0 },
    new ChartOfAccountType { Id = 13, Name = "Input Tax", ParentId = 1, GroupID = 0 },

    // Child Accounts under "Liabilities" (ParentId = 2)
    new ChartOfAccountType { Id = 14, Name = "Other Current Liability", ParentId = 2, GroupID = 0 },
    new ChartOfAccountType { Id = 15, Name = "Credit Card", ParentId = 2, GroupID = 0 },
    new ChartOfAccountType { Id = 16, Name = "Long Term Liability", ParentId = 2, GroupID = 0 },
    new ChartOfAccountType { Id = 17, Name = "Other Liability", ParentId = 2, GroupID = 0 },
    new ChartOfAccountType { Id = 18, Name = "Overseas Tax Payable", ParentId = 2, GroupID = 0 },
    new ChartOfAccountType { Id = 19, Name = "Output Tax", ParentId = 2, GroupID = 0 },

    // Child Accounts under "Equity" (ParentId = 3)
    new ChartOfAccountType { Id = 20, Name = "Equity", ParentId = 3, GroupID = 0 },

    // Child Accounts under "Income" (ParentId = 4)
    new ChartOfAccountType { Id = 21, Name = "Income", ParentId = 4, GroupID = 0 },
    new ChartOfAccountType { Id = 22, Name = "Other Income", ParentId = 4, GroupID = 0 },

    // Child Accounts under "Expense" (ParentId = 5)
    new ChartOfAccountType { Id = 23, Name = "Expense", ParentId = 5, GroupID = 0 },
    new ChartOfAccountType { Id = 24, Name = "Cost of Goods Sold", ParentId = 5, GroupID = 0 },
    new ChartOfAccountType { Id = 25, Name = "Other Expense", ParentId = 5, GroupID = 0 }
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

            modelBuilder.Entity<ChartOfAccount>().HasOne(ca => ca.Subscription).WithMany().HasForeignKey(ca => ca.SubScriptionId).OnDelete(DeleteBehavior.Restrict);


            // Decimal property configuration to avoid truncation warnings
            modelBuilder.Entity<JournalPost>()
                .Property(j => j.Credit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<JournalPost>()
                .Property(j => j.Debit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<JournalPostDetail>()
                .Property(j => j.Credit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<JournalPostDetail>()
                .Property(j => j.Debit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OpeningBalances>()
                .Property(o => o.ClsCredit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OpeningBalances>()
                .Property(o => o.ClsDebit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OpeningBalances>()
                .Property(o => o.OpnCredit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OpeningBalances>()
                .Property(o => o.OpnDebit)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OpeningBalances>()
            .Property(o => o.Debit)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OpeningBalances>()
            .Property(o => o.Credit)
            .HasColumnType("decimal(18,2)");

        }

    }
}
