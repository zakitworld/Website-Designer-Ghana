using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class BlogPosts
{
    [Inject] private IBlogService BlogService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private IEnumerable<Website_Designer_Ghana.Data.Models.BlogPost>? blogPosts;
    private bool showSuccessMessage = false;
    private string successMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            blogPosts = (await BlogService.GetAllPostsAsync(publishedOnly: false))
            .OrderByDescending(p => p.CreatedAt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading blog posts: {ex.Message}");
            blogPosts = new List<Website_Designer_Ghana.Data.Models.BlogPost>();
        }
    }

    private async Task DeletePost(int postId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this blog post?"))
        {
            try
            {
                await BlogService.DeletePostAsync(postId);
                successMessage = "Blog post deleted successfully.";
                showSuccessMessage = true;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting blog post: {ex.Message}");
            }
        }
    }
}
