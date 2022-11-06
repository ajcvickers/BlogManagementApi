using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi_Net7;

[ApiController]
public class PostsController : ControllerBase
{
    private readonly BlogsContext _context;

    public PostsController(BlogsContext context)
    {
        _context = context;
    }

    [HttpGet("api/posts")]
    public async Task<IEnumerable<Post>> GetPosts()
    {
        return await _context.Posts
            .Include(p => p.Blog)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet("api/posts/{id}")]
    public async Task<ActionResult<Post>> GetPost(int id)
    {
        var post = await _context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return post == null ? NotFound() : Ok(post);
    }

    [HttpPost("api/posts")]
    public async Task<ActionResult<Post>> InsertPost(Post post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return Ok(post);
    }

    [HttpPut("api/posts")]
    public async Task<ActionResult<Post>> UpdatePost(Post post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(post).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(post);
    }

    [HttpDelete("api/posts/{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        var rowsAffected = await _context.Posts.Where(p => p.Id == id).ExecuteDeleteAsync();

        if (rowsAffected == 0)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPut("api/posts/archive")]
    public async Task<ActionResult> ArchivePosts(string blogName, int priorToYear)
    {
        var priorToDateTime = new DateTime(priorToYear, 1, 1);

        await _context.Posts
            .Where(
                p => p.Blog.Name == blogName
                    && p.Blog.Account.Details.IsPremium == false
                    && p.PublishedOn < priorToDateTime
                    && !p.Archived)
            .ExecuteUpdateAsync(
                updates => updates
                    .SetProperty(p => p.Title, p => p.Title + " (" + p.PublishedOn.Year + ")")
                    .SetProperty(p => p.Banner, p => "This post was published in " + p.PublishedOn.Year + " and has been archived.")
                    .SetProperty(p => p.Archived, true));

        return Ok();
    }
}
