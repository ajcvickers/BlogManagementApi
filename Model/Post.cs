using System.ComponentModel.DataAnnotations;

public class Post
{
    public int Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string Title { get; set; }

    [MaxLength(128)]
    public string Banner { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime PublishedOn { get; set; }
    public bool Archived { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}
