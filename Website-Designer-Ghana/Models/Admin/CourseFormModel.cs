using System.ComponentModel.DataAnnotations;

namespace Website_Designer_Ghana.Models.Admin;

public class CourseFormModel
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

    public string? FeaturedImage { get; set; }
    public string? Icon { get; set; }

    [Required]
    public decimal Price { get; set; } = 0;

    public decimal? DiscountPrice { get; set; }
    public string? Duration { get; set; }
    public string? Level { get; set; }
    public bool IsPublished { get; set; }
}
