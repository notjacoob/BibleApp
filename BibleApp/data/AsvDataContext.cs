using BibleApp.Models;
using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.data;

/// <summary>
/// chapter -> verse data context
/// </summary>
/// <param name="configuration"></param>
public class AsvDataContext(DbAccessor accessor) : DbContext
{
    /// <summary>
    /// Database set of verse models
    /// </summary>
    public DbSet<VerseModel> Verses { get; set; }
    /// <summary>
    /// configure database connection
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = accessor.GetConnectionString();
        optionsBuilder.UseSqlServer(conn);
    }
    /// <summary>
    /// map foreign key relationships
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VerseModel>().HasOne(e => e.Book).WithMany(e => e.Verses).HasForeignKey(e => e.VerseBookId)
            .HasPrincipalKey(e => e.BookId);
    }
}