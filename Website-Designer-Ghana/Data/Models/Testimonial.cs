using System.ComponentModel.DataAnnotations;

namespace Website_Designer_Ghana.Data.Models;

public class Testimonial
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ClientName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ClientPosition { get; set; }

    [MaxLength(200)]
    public string? ClientCompany { get; set; }

    [MaxLength(500)]
    public string? ClientImage { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [Required]
    public bool IsPublished { get; set; } = false;

    [Required]
    public bool IsFeatured { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; }

    public int OrderIndex { get; set; } = 0;
}
