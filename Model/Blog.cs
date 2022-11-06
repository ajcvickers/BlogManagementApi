using System.ComponentModel.DataAnnotations;

public class Blog
{
    public int Id { get; set; }

    [Required]
    [MaxLength(64)]
    public string Name { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }

    public List<Post> Posts { get; set; } = new();
}
