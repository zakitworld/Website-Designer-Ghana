using Microsoft.AspNetCore.Components;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class Dashboard
{
    [Inject] private IBlogService BlogService { get; set; } = default!;
    [Inject] private IPortfolioService PortfolioService { get; set; } = default!;
    [Inject] private IContactService ContactService { get; set; } = default!;
    [Inject] private ILogger<Dashboard> Logger { get; set; } = default!;

    private IEnumerable<Website_Designer_Ghana.Data.Models.BlogPost>? blogPosts;
    private IEnumerable<Website_Designer_Ghana.Data.Models.Portfolio>? portfolios;
    private IEnumerable<Website_Designer_Ghana.Data.Models.ContactSubmission>? contactSubmissions;
    private bool isLoading = true;
    private string? errorMessage;
    private int TotalPosts => blogPosts?.Count() ?? 0;
    private int PublishedPosts => blogPosts?.Count(x => x.IsPublished) ?? 0;
    private int DraftPosts => TotalPosts - PublishedPosts;
    private int TotalViews => blogPosts?.Sum(x => x.ViewCount) ?? 0;
    private int TotalPortfolios => portfolios?.Count() ?? 0;
    private int TotalSubmissions => contactSubmissions?.Count() ?? 0;
    private int UnreadSubmissions => contactSubmissions?.Count(x => !x.IsRead) ?? 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        isLoading = true;
        errorMessage = null;
        try
        {
            // These services share the scoped EF Core DbContext in this circuit, so
            // their queries must not overlap on the same context instance.
            blogPosts = (await BlogService.GetAllPostsAsync(publishedOnly: false))
                .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
                .ToList();
            portfolios = (await PortfolioService.GetAllPortfoliosAsync(publishedOnly: false))
                .ToList();
            contactSubmissions = (await ContactService.GetRecentSubmissionsAsync(20))
                .OrderByDescending(s => s.SubmittedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load the admin dashboard");
            errorMessage = "Check the database connection and try again.";
        }
        finally
        {
            isLoading = false;
        }
    }

    private static string GetInitials(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Take(2).Select(x => char.ToUpperInvariant(x[0])));
    }
}
