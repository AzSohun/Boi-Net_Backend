using Boi.Net.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Boi.Net.Data
{
    public class BoiNetDbContext: IdentityDbContext<User>
    {

        public BoiNetDbContext(DbContextOptions<BoiNetDbContext> options): base(options)
        {
            
        }

        public DbSet<Book> Books { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // ==========================================================
            // EF Core Fluent API Configurations
            // ==========================================================

            // Price কলামটিকে ডাটাবেজে decimal(18,2) হিসেবে ফোর্স করা হচ্ছে।
            // কারণ: Double টাইপ ব্যবহার করলে Precision Loss এবং InvalidCastException হতে পারে।
            modelBuilder.Entity<Book>()
                .Property(book => book.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<User>()
                .Property(user => user.UserRole)
                .HasConversion<string>();
        }

    }


}
