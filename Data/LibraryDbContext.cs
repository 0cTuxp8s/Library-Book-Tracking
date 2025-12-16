using Microsoft.EntityFrameworkCore;
using LibraryBookTracking.Models;

namespace LibraryBookTracking.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=library.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Book entity
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasIndex(e => e.Title);
                entity.HasIndex(e => e.Author);
            });

            // Configure BorrowRecord entity
            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.HasOne(b => b.Book)
                      .WithMany(b => b.BorrowRecords)
                      .HasForeignKey(b => b.BookId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

