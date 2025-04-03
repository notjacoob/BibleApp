using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.data;

public class BooksDataContext(IConfiguration configuration) : DbContext
{
    public DbSet<BookModel> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = configuration["ConnectionStrings:local"];
        optionsBuilder.UseSqlServer(conn);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookModel>().HasMany(e => e.Verses).WithOne(e => e.Book).HasForeignKey(e => e.VerseBookId)
            .HasPrincipalKey(e => e.BookId);
    }
}