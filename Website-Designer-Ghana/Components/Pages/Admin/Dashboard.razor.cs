using Microsoft.AspNetCore.Components;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class Dashboard
{
    [Inject] private IBlogService BlogService { get; set; } = default!;
    [Inject] private IPortfolioService PortfolioService { get; set; } = default!;
    [Inject] private IContactService ContactService { get; set; } = default!;

    private IEnumerable<Website_Designer_Ghana.Data.Models.BlogPost>? blogPosts;
    private IEnumerable<Website_Designer_Ghana.Data.Models.Portfolio>? portfolios;
    private IEnumerable<Website_Designer_Ghana.Data.Models.ContactSubmission>? contactSubmissions;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            blogPosts = await BlogService.GetAllPostsAsync(publishedOnly: false);
            portfolios = await PortfolioService.GetAllPortfoliosAsync(publishedOnly: false);
            contactSubmissions = (await ContactService.GetRecentSubmissionsAsync(20))
            .OrderByDescending(s => s.SubmittedAt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
    }
}
