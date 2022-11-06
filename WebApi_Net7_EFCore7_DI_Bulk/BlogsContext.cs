using Microsoft.EntityFrameworkCore;

namespace WebApi_Net7;

public class BlogsContext : DbContext
{
    public BlogsContext(DbContextOptions<BlogsContext> options)
        : base(options)
    {
    }

    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Blog>()
            .HasIndex(e => e.Name);

        modelBuilder
            .Entity<Account>()
            .OwnsOne(e => e.Details)
            .ToJson();

        // Ignore the string property now we are mapping the JSON object
        modelBuilder
            .Entity<Account>()
            .Ignore(e => e.DetailsJson);
    }
}
