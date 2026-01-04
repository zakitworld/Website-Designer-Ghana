using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Data.Repositories;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Services.Implementations;

public class ContactService : IContactService
{
    private readonly IRepository<ContactSubmission> _repository;

    public ContactService(IRepository<ContactSubmission> repository)
    {
        _repository = repository;
    }

    public async Task<ContactSubmission> SubmitContactFormAsync(ContactSubmission submission)
    {
        submission.SubmittedAt = DateTime.UtcNow;
        submission.IsRead = false;
        submission.IsReplied = false;
        return await _repository.AddAsync(submission);
    }

    public async Task<IEnumerable<ContactSubmission>> GetAllSubmissionsAsync()
    {
        var submissions = await _repository.GetAllAsync();
        return submissions.OrderByDescending(s => s.SubmittedAt);
    }

    public async Task<(IEnumerable<ContactSubmission> Submissions, int TotalCount)> GetPagedSubmissionsAsync(
        int pageNumber,
        int pageSize,
        bool? isRead = null)
    {
        return await _repository.GetPagedAsync(
            pageNumber,
            pageSize,
            filter: isRead.HasValue ? s => s.IsRead == isRead.Value : null,
            orderBy: q => q.OrderByDescending(s => s.SubmittedAt));
    }

    public async Task<ContactSubmission?> GetSubmissionByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task MarkAsReadAsync(int id)
    {
        var submission = await _repository.GetByIdAsync(id);
        if (submission != null && !submission.IsRead)
        {
            submission.IsRead = true;
            submission.ReadAt = DateTime.UtcNow;
            await _repository.UpdateAsync(submission);
        }
    }

    public async Task MarkAsRepliedAsync(int id)
    {
        var submission = await _repository.GetByIdAsync(id);
        if (submission != null && !submission.IsReplied)
        {
            submission.IsReplied = true;
            submission.RepliedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(submission);
        }
    }

    public async Task UpdateAdminNotesAsync(int id, string notes)
    {
        var submission = await _repository.GetByIdAsync(id);
        if (submission != null)
        {
            submission.AdminNotes = notes;
            await _repository.UpdateAsync(submission);
        }
    }

    public async Task DeleteSubmissionAsync(int id)
    {
        var submission = await _repository.GetByIdAsync(id);
        if (submission != null)
        {
            await _repository.DeleteAsync(submission);
        }
    }

    public async Task<int> GetUnreadCountAsync()
    {
        return await _repository.CountAsync(s => !s.IsRead);
    }

    public async Task<IEnumerable<ContactSubmission>> GetRecentSubmissionsAsync(int count)
    {
        var allSubmissions = await _repository.GetAllAsync();
        return allSubmissions.OrderByDescending(s => s.SubmittedAt).Take(count);
    }
}
