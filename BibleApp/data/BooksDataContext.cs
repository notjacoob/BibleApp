using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.data;

/// <summary>
/// book data context
/// </summary>
/// <param name="configuration"></param>
public class BooksDataContext(IConfiguration configuration) : DbContext
{
    /// <summary>
    /// Database set of book models
    /// </summary>
    public DbSet<BookModel> Books { get; set; }
    /// <summary>
    /// configure database connection
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = configuration["ConnectionStrings:local"];
        optionsBuilder.UseSqlServer(conn);
    }
    /// <summary>
    /// map foreign key relationships
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookModel>().HasMany(e => e.Verses).WithOne(e => e.Book).HasForeignKey(e => e.VerseBookId)
            .HasPrincipalKey(e => e.BookId);
    }
}