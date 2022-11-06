using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        var post = await _context.Posts.Where(p => p.Id == id).FirstOrDefaultAsync();

        if (post == null)
        {
            return NotFound();
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("api/posts/archive")]
    public async Task<ActionResult> ArchivePosts(string blogName, int priorToYear)
    {
        var priorToDateTime = new DateTime(priorToYear, 1, 1);

        var posts = await _context.Posts
            .Include(p => p.Blog.Account)
            .Where(
                p => p.Blog.Name == blogName
                    && p.PublishedOn < priorToDateTime
                    && !p.Archived)
            .ToListAsync();

        foreach (var post in posts)
        {
            var accountDetails = JsonConvert.DeserializeObject<AccountDetails>(post.Blog.Account.DetailsJson)!;
            if (!accountDetails.IsPremium)
            {
                post.Archived = true;
                post.Banner = $"This post was published in {post.PublishedOn.Year} and has been archived.";
                post.Title += $" ({post.PublishedOn.Year})";
            }
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}
