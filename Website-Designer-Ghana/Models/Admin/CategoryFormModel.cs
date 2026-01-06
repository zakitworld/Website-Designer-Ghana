using System.ComponentModel.DataAnnotations;

namespace Website_Designer_Ghana.Models.Admin;

public class CategoryFormModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]
    [MaxLength(150)]
    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Color { get; set; } = "#3b82f6";
}
