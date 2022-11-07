using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using WebApi_Net7;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<BlogsContext>(
    b =>
    {
        b.UseSqlServer("name=Blogs");
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet(
    "api/posts", async (BlogsContext context) =>
        await context.Posts.Include(p => p.Blog).AsNoTracking().ToListAsync());

app.MapGet(
    "api/posts/{id}", async (int id, BlogsContext context) =>
    {
        var post = await context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        return post == null ? Results.NotFound() : Results.Ok(post);
    });

app.MapPost(
    "api/posts", async (Post post, BlogsContext context) =>
    {
        context.Posts.Add(post);
        await context.SaveChangesAsync();

        return Results.Ok(post);
    });

app.MapPut(
    "api/posts", async (Post post, BlogsContext context) =>
        await context.Posts.Where(p => p.Id == post.Id).ExecuteUpdateAsync(
            updates => updates
                .SetProperty(p => p.Title, post.Title)
                .SetProperty(p => p.Banner, post.Banner)
                .SetProperty(p => p.Content, post.Content)
                .SetProperty(p => p.Archived, post.Archived)
                .SetProperty(p => p.PublishedOn, post.PublishedOn)) == 0
            ? Results.NotFound()
            : Results.Ok(post));

app.MapDelete(
    "api/posts/{id}",
    async (int id, BlogsContext context)
        => await context.Posts.Where(p => p.Id == id).ExecuteDeleteAsync() == 0
            ? Results.NotFound()
            : Results.Ok());

app.MapPut(
    "api/posts/archive", async (string blogName, int priorToYear, BlogsContext context) =>
    {
        var priorToDateTime = new DateTime(priorToYear, 1, 1);

        await context.Posts
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

        return Results.Ok();
    });

app.Run();
