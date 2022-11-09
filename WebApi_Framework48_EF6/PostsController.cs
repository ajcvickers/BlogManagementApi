using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;

namespace WebApi_Framework48_EF6;

public class PostsController : ApiController
{
    [HttpGet]
    [Route("api/posts")]
    public async Task<IEnumerable<Post>> GetPosts()
    {
        using var context = new BlogsContext();

        return await context.Posts
            .Include(p => p.Blog)
            .AsNoTracking()
            .ToListAsync();
    }

    [HttpGet]
    [Route("api/posts/{id}")]
    [ResponseType(typeof(Post))]
    public async Task<IHttpActionResult> GetPost(int id)
    {
        using var context = new BlogsContext();

        var post = await context.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return post == null ? NotFound() : Ok(post);
    }

    [HttpPost]
    [Route("api/posts")]
    [ResponseType(typeof(Post))]
    public async Task<IHttpActionResult> InsertPost(Post post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var context = new BlogsContext();

        context.Posts.Add(post);
        await context.SaveChangesAsync();

        return Ok(post);
    }

    [HttpPut]
    [Route("api/posts")]
    [ResponseType(typeof(Post))]
    public async Task<IHttpActionResult> UpdatePost(Post post)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        using var context = new BlogsContext();

        var existingPost = await context.Posts.Where(p => p.Id == post.Id).FirstOrDefaultAsync();

        if (existingPost == null)
        {
            return NotFound();
        }

        context.Entry(existingPost).CurrentValues.SetValues(post);

        await context.SaveChangesAsync();

        return Ok(post);
    }

    [HttpDelete]
    [Route("api/posts/{id}")]
    public async Task<IHttpActionResult> DeletePost(int id)
    {
        using var context = new BlogsContext();

        var post = await context.Posts.Where(p => p.Id == id).FirstOrDefaultAsync();

        if (post == null)
        {
            return NotFound();
        }

        context.Posts.Remove(post);
        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut]
    [Route("api/posts/archive")]
    public async Task<IHttpActionResult> ArchivePosts(string blogName, int priorToYear)
    {
        var priorToDateTime = new DateTime(priorToYear, 1, 1);

        using var context = new BlogsContext();

        var posts = await context.Posts
            .Include(p => p.Blog.Account)
            .Where(
                p => p.Blog.Name == blogName
                    && p.PublishedOn < priorToDateTime
                    && !p.Archived)
            .ToListAsync();

        foreach (var post in posts)
        {
            var accountDetails = JsonConvert.DeserializeObject<AccountDetails>(post.Blog.Account.Details)!;
            if (!accountDetails.IsPremium)
            {
                post.Archived = true;
                post.Banner = $"This post was published in {post.PublishedOn.Year} and has been archived.";
                post.Title += $" ({post.PublishedOn.Year})";
            }
        }

        await context.SaveChangesAsync();

        return Ok();
    }
}
