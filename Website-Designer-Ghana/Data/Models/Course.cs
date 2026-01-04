using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website_Designer_Ghana.Data.Models;

public class Course
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

    [MaxLength(500)]
    public string? FeaturedImage { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPrice { get; set; }

    [Required]
    [MaxLength(3)]
    public string Currency { get; set; } = "GHS";

    [MaxLength(50)]
    public string? Duration { get; set; }

    [MaxLength(50)]
    public string? Level { get; set; }

    [Required]
    public bool IsPublished { get; set; } = false;

    [Required]
    public bool IsFeatured { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PublishedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int EnrollmentCount { get; set; } = 0;

    [MaxLength(200)]
    public string? MetaTitle { get; set; }

    [MaxLength(500)]
    public string? MetaDescription { get; set; }

    // Navigation Properties
    public ICollection<CourseLesson> Lessons { get; set; } = new List<CourseLesson>();
}
