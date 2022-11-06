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

        // Ignore mapped JSON object until change to map it in WebApi_Net7_EFCore7_DI_Bulk
        modelBuilder
            .Entity<Account>()
            .Ignore(e => e.Details);

        // Instead map the DetailsJson string property to the JSON column
        modelBuilder
            .Entity<Account>()
            .Property(e => e.DetailsJson)
            .HasColumnName("Details");
    }
}
