using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.data;
/// <summary>
/// verse -> comments data context
/// </summary>
/// <param name="configuration"></param>
public class CommentsDataContext(IConfiguration configuration) : DbContext
{
    /// <summary>
    /// Database set of comment models
    /// </summary>
    public DbSet<CommentModel> Comments { get; set; }
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
        modelBuilder.Entity<CommentModel>().HasOne(e => e.Verse).WithMany(e => e.Comments).HasForeignKey(e => e.VerseId)
            .HasPrincipalKey(e => e.Id);
    }
}