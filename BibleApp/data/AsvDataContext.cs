using BibleApp.Models;
using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.data;

public class AsvDataContext(IConfiguration configuration) : DbContext
{
    public DbSet<VerseModel> Verses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = configuration["ConnectionStrings:local"];
        optionsBuilder.UseSqlServer(conn);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VerseModel>().HasOne(e => e.Book).WithMany(e => e.Verses).HasForeignKey(e => e.VerseBookId)
            .HasPrincipalKey(e => e.BookId);
    }
}