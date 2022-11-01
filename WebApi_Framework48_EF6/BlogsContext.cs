using System.Data.Entity;

namespace WebApi_Framework48_EF6;

public class BlogsContext : DbContext
{
    public BlogsContext()
        : base("name=Blogs")
    {
    }
        
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }
}
