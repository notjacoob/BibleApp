using BibleApp.data;
using BibleApp.Models.DB;
using BibleApp.Models.View;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.util;

public class CommentUtil(IConfiguration config)
{
    public async Task<CommentModel> Create(AddCommentModel viewModel)
    {
        await using var ctx = new CommentsDataContext(config);
        var c = new CommentModel{Content = viewModel.Content, CreatedAt = DateTime.Now, VerseId = viewModel.VerseId};
        ctx.Add(c);
        await ctx.SaveChangesAsync();
        return c;
    }

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