using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website_Designer_Ghana.Data.Models;

public class Portfolio
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
    public string Description { get; set; } = string.Empty;

    public string? FullDescription { get; set; }

    [Required]
    [MaxLength(500)]
    public string FeaturedImage { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ClientName { get; set; }

    [MaxLength(500)]
    public string? ClientWebsite { get; set; }

    [MaxLength(500)]
    public string? ProjectUrl { get; set; }

    public DateTime? CompletedDate { get; set; }

    [MaxLength(1000)]
    public string? Technologies { get; set; }

    [Required]
    public bool IsPublished { get; set; } = false;

    [Required]
    public bool IsFeatured { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int ViewCount { get; set; } = 0;

    // Navigation Properties
    public int? CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public PortfolioCategory? Category { get; set; }
}
