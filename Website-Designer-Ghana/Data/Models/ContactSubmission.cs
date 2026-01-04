using System.ComponentModel.DataAnnotations;

namespace Website_Designer_Ghana.Data.Models;

public class ContactSubmission
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Company { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsRead { get; set; } = false;

    public DateTime? ReadAt { get; set; }

    [Required]
    public bool IsReplied { get; set; } = false;

    public DateTime? RepliedAt { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [MaxLength(2000)]
    public string? AdminNotes { get; set; }
}
