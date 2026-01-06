using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class BlogComments
{
    [Inject] private IBlogService BlogService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private IEnumerable<Website_Designer_Ghana.Data.Models.BlogComment>? comments;
    private bool showSuccessMessage = false;
    private string successMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadCommentsAsync();
    }

    private async Task LoadCommentsAsync()
    {
        try
        {
            comments = await BlogService.GetAllCommentsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading comments: {ex.Message}");
            comments = new List<Website_Designer_Ghana.Data.Models.BlogComment>();
        }
    }

    private async Task ApproveComment(int commentId)
    {
        try
        {
            await BlogService.ApproveCommentAsync(commentId);
            successMessage = "Comment approved successfully.";
            showSuccessMessage = true;
            await LoadCommentsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error approving comment: {ex.Message}");
        }
    }

    private async Task DeleteComment(int commentId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this comment?"))
        {
            try
            {
                await BlogService.DeleteCommentAsync(commentId);
                successMessage = "Comment deleted successfully.";
                showSuccessMessage = true;
                await LoadCommentsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
            }
        }
    }
}
