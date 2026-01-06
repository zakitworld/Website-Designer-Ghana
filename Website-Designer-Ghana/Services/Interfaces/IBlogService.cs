using Website_Designer_Ghana.Data.Models;

namespace Website_Designer_Ghana.Services.Interfaces;

public interface IBlogService
{
    // Blog Posts
    Task<BlogPost?> GetPostByIdAsync(int id);
    Task<BlogPost?> GetPostBySlugAsync(string slug);
    Task<IEnumerable<BlogPost>> GetAllPostsAsync(bool publishedOnly = true);
    Task<(IEnumerable<BlogPost> Posts, int TotalCount)> GetPagedPostsAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        bool publishedOnly = true);
    Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 5, bool publishedOnly = true);
    Task<IEnumerable<BlogPost>> GetFeaturedPostsAsync(bool publishedOnly = true);
    Task<IEnumerable<BlogPost>> SearchPostsAsync(string searchTerm, bool publishedOnly = true);
    Task<BlogPost> CreatePostAsync(BlogPost post);
    Task UpdatePostAsync(BlogPost post);
    Task DeletePostAsync(int id);
    Task IncrementViewCountAsync(int postId);

    // Categories
    Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync();
    Task<BlogCategory?> GetCategoryByIdAsync(int id);
    Task<BlogCategory?> GetCategoryBySlugAsync(string slug);
    Task<BlogCategory> CreateCategoryAsync(BlogCategory category);
    Task UpdateCategoryAsync(BlogCategory category);
    Task DeleteCategoryAsync(int id);

    // Comments
    Task<IEnumerable<BlogComment>> GetPostCommentsAsync(int postId, bool approvedOnly = true);
    Task<IEnumerable<BlogComment>> GetAllCommentsAsync(bool approvedOnly = false);
    Task<BlogComment> AddCommentAsync(BlogComment comment);
    Task ApproveCommentAsync(int commentId);
    Task DeleteCommentAsync(int commentId);

    // Tags
    Task<IEnumerable<BlogTag>> GetAllTagsAsync();
    Task<BlogTag?> GetTagBySlugAsync(string slug);
    Task<BlogTag> CreateTagAsync(BlogTag tag);
    Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagSlug, bool publishedOnly = true);
}
