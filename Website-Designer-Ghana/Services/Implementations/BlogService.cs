using Microsoft.EntityFrameworkCore;
using Website_Designer_Ghana.Data;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Data.Repositories;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Services.Implementations;

public class BlogService : IBlogService
{
    private readonly IRepository<BlogPost> _postRepository;
    private readonly IRepository<BlogCategory> _categoryRepository;
    private readonly IRepository<BlogComment> _commentRepository;
    private readonly IRepository<BlogTag> _tagRepository;
    private readonly ApplicationDbContext _context;

    public BlogService(
        IRepository<BlogPost> postRepository,
        IRepository<BlogCategory> categoryRepository,
        IRepository<BlogComment> commentRepository,
        IRepository<BlogTag> tagRepository,
        ApplicationDbContext context)
    {
        _postRepository = postRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _tagRepository = tagRepository;
        _context = context;
    }

    // Blog Posts
    public async Task<BlogPost?> GetPostByIdAsync(int id)
    {
        return await _context.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.Comments.Where(c => c.IsApproved))
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<BlogPost?> GetPostBySlugAsync(string slug)
    {
        return await _context.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.Comments.Where(c => c.IsApproved))
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<BlogPost>> GetAllPostsAsync(bool publishedOnly = true)
    {
        var query = _context.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<(IEnumerable<BlogPost> Posts, int TotalCount)> GetPagedPostsAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        bool publishedOnly = true)
    {
        var query = _context.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        var totalCount = await query.CountAsync();

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (posts, totalCount);
    }

    public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count = 5, bool publishedOnly = true)
    {
        var query = _context.BlogPosts
            .Include(p => p.Category)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> GetFeaturedPostsAsync(bool publishedOnly = true)
    {
        var query = _context.BlogPosts
            .Include(p => p.Category)
            .Where(p => p.ViewCount > 100) // Example: Posts with >100 views are "featured"
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query
            .OrderByDescending(p => p.ViewCount)
            .Take(3)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlogPost>> SearchPostsAsync(string searchTerm, bool publishedOnly = true)
    {
        var query = _context.BlogPosts
            .Include(p => p.Category)
            .Where(p => p.Title.Contains(searchTerm) ||
                       p.Summary.Contains(searchTerm) ||
                       p.Content.Contains(searchTerm))
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<BlogPost> CreatePostAsync(BlogPost post)
    {
        post.CreatedAt = DateTime.UtcNow;
        if (post.IsPublished && post.PublishedAt == null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        return await _postRepository.AddAsync(post);
    }

    public async Task UpdatePostAsync(BlogPost post)
    {
        post.UpdatedAt = DateTime.UtcNow;
        if (post.IsPublished && post.PublishedAt == null)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        await _postRepository.UpdateAsync(post);
    }

    public async Task DeletePostAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);
        if (post != null)
        {
            await _postRepository.DeleteAsync(post);
        }
    }

    public async Task IncrementViewCountAsync(int postId)
    {
        var post = await _postRepository.GetByIdAsync(postId);
        if (post != null)
        {
            post.ViewCount++;
            await _postRepository.UpdateAsync(post);
        }
    }

    // Categories
    public async Task<IEnumerable<BlogCategory>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<BlogCategory?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<BlogCategory?> GetCategoryBySlugAsync(string slug)
    {
        return await _categoryRepository.FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<BlogCategory> CreateCategoryAsync(BlogCategory category)
    {
        category.CreatedAt = DateTime.UtcNow;
        return await _categoryRepository.AddAsync(category);
    }

    public async Task UpdateCategoryAsync(BlogCategory category)
    {
        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category != null)
        {
            await _categoryRepository.DeleteAsync(category);
        }
    }

    // Comments
    public async Task<IEnumerable<BlogComment>> GetPostCommentsAsync(int postId, bool approvedOnly = true)
    {
        var query = _context.BlogComments
            .Where(c => c.BlogPostId == postId && c.ParentCommentId == null)
            .Include(c => c.Replies)
            .AsQueryable();

        if (approvedOnly)
        {
            query = query.Where(c => c.IsApproved);
        }

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<BlogComment> AddCommentAsync(BlogComment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;
        comment.IsApproved = false; // Require approval
        return await _commentRepository.AddAsync(comment);
    }

    public async Task ApproveCommentAsync(int commentId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment != null)
        {
            comment.IsApproved = true;
            await _commentRepository.UpdateAsync(comment);
        }
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment != null)
        {
            await _commentRepository.DeleteAsync(comment);
        }
    }

    // Tags
    public async Task<IEnumerable<BlogTag>> GetAllTagsAsync()
    {
        return await _tagRepository.GetAllAsync();
    }

    public async Task<BlogTag?> GetTagBySlugAsync(string slug)
    {
        return await _tagRepository.FirstOrDefaultAsync(t => t.Slug == slug);
    }

    public async Task<BlogTag> CreateTagAsync(BlogTag tag)
    {
        tag.CreatedAt = DateTime.UtcNow;
        return await _tagRepository.AddAsync(tag);
    }

    public async Task<IEnumerable<BlogPost>> GetPostsByTagAsync(string tagSlug, bool publishedOnly = true)
    {
        var query = _context.BlogPosts
            .Include(p => p.Category)
            .Include(p => p.Tags)
            .Where(p => p.Tags.Any(t => t.Slug == tagSlug))
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }
}
