using Microsoft.AspNetCore.Components;
using Website_Designer_Ghana.Services.Interfaces;
using Website_Designer_Ghana.Data.Models;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class ContactSubmissions
{
    [Inject] private IContactService ContactService { get; set; } = default!;

    private IEnumerable<ContactSubmission>? submissions;
    private ContactSubmission? selectedSubmission;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            submissions = (await ContactService.GetRecentSubmissionsAsync(100))
            .OrderByDescending(s => s.SubmittedAt);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading contact submissions: {ex.Message}");
            submissions = new List<ContactSubmission>();
        }
    }

    private async Task ViewSubmission(ContactSubmission submission)
    {
        selectedSubmission = submission;

        if (!submission.IsRead)
        {
            try
            {
                await ContactService.MarkAsReadAsync(submission.Id);
                submission.IsRead = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking submission as read: {ex.Message}");
            }
        }
    }

    private void CloseModal()
    {
        selectedSubmission = null;
    }
}
