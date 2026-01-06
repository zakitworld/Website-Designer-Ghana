using System.ComponentModel.DataAnnotations;

namespace Website_Designer_Ghana.Models.Admin;

public class PortfolioFormModel
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FeaturedImage { get; set; } = string.Empty;

    public string? ClientName { get; set; }
    public string? ProjectUrl { get; set; }
    public string? Technologies { get; set; }
    public int? CategoryId { get; set; }
    public bool IsPublished { get; set; }
}
