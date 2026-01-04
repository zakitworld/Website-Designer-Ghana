using Website_Designer_Ghana.Data.Models;

namespace Website_Designer_Ghana.Services.Interfaces;

public interface IContactService
{
    Task<ContactSubmission> SubmitContactFormAsync(ContactSubmission submission);
    Task<IEnumerable<ContactSubmission>> GetAllSubmissionsAsync();
    Task<(IEnumerable<ContactSubmission> Submissions, int TotalCount)> GetPagedSubmissionsAsync(
        int pageNumber,
        int pageSize,
        bool? isRead = null);
    Task<ContactSubmission?> GetSubmissionByIdAsync(int id);
    Task MarkAsReadAsync(int id);
    Task MarkAsRepliedAsync(int id);
    Task UpdateAdminNotesAsync(int id, string notes);
    Task DeleteSubmissionAsync(int id);
    Task<int> GetUnreadCountAsync();
    Task<IEnumerable<ContactSubmission>> GetRecentSubmissionsAsync(int count);
}
