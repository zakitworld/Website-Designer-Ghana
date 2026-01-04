using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website_Designer_Ghana.Data.Models;

public class BlogPost
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? FeaturedImage { get; set; }

    [Required]
    public bool IsPublished { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    public int ViewCount { get; set; } = 0;

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    [MaxLength(500)]
    public string? MetaKeywords { get; set; }

    // Navigation Properties
    public int? CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public BlogCategory? Category { get; set; }

    public ICollection<BlogComment> Comments { get; set; } = new List<BlogComment>();
    public ICollection<BlogTag> Tags { get; set; } = new List<BlogTag>();
}
