using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website_Designer_Ghana.Data.Models;

public class BlogComment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BlogPostId { get; set; }

    [ForeignKey(nameof(BlogPostId))]
    public BlogPost BlogPost { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string AuthorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string AuthorEmail { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? AuthorWebsite { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsApproved { get; set; } = false;

    public int? ParentCommentId { get; set; }

    [ForeignKey(nameof(ParentCommentId))]
    public BlogComment? ParentComment { get; set; }

    // Navigation Properties
    public ICollection<BlogComment> Replies { get; set; } = new List<BlogComment>();
}
