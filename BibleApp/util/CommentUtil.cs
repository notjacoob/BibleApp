using BibleApp.data;
using BibleApp.Models.DB;
using BibleApp.Models.View;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.util;
/// <summary>
/// comment business layer
/// </summary>
/// <param name="config">injected</param>
public class CommentUtil(IConfiguration config)
{
    /// <summary>
    /// Create a comment
    /// </summary>
    /// <param name="viewModel">the view model</param>
    /// <returns>the model of the new comment</returns>
    public async Task<CommentModel> Create(AddCommentModel viewModel)
    {
        await using var ctx = new CommentsDataContext(config);
        var c = new CommentModel{Content = viewModel.Content, CreatedAt = DateTime.Now, VerseId = viewModel.VerseId};
        ctx.Add(c);
        await ctx.SaveChangesAsync();
        return c;
    }
    /// <summary>
    /// Delete a comment
    /// </summary>
    /// <param name="commentId">the ID of the comment to delete</param>
    public async Task Delete(int commentId)
    {
        await using var ctx = new CommentsDataContext(config);
        var model = await ctx.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
        if (model != null)
        {
            ctx.Comments.Remove(model);
            await ctx.SaveChangesAsync();
        }
    }
}