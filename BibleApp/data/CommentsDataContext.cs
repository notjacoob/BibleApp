using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.data;

public class CommentsDataContext(IConfiguration configuration) : DbContext
{
    public DbSet<CommentModel> Comments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conn = configuration["ConnectionStrings:local"];
        optionsBuilder.UseSqlServer(conn);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommentModel>().HasOne(e => e.Verse).WithMany(e => e.Comments).HasForeignKey(e => e.VerseId)
            .HasPrincipalKey(e => e.Id);
    }
}