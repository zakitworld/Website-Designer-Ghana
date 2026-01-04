using System.ComponentModel.DataAnnotations;

namespace Website_Designer_Ghana.Data.Models;

public class BlogTag
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(75)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ICollection<BlogPost> Posts { get; set; } = new List<BlogPost>();
}
