using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Website_Designer_Ghana.Data;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Data.Repositories;
using Website_Designer_Ghana.Services.Implementations;
using Xunit;

namespace Website_Designer_Ghana.Tests.Services;

public class BlogServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<BlogService>> _loggerMock;
    private readonly BlogService _blogService;

    public BlogServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<BlogService>>();

        // Create repository and service
        var repository = new Repository<BlogPost>(_context);
        _blogService = new BlogService(
            repository,
            new Repository<BlogCategory>(_context),
            new Repository<BlogComment>(_context),
            new Repository<BlogTag>(_context),
            _loggerMock.Object
        );

        // Seed test data
        SeedTestData();
    }

    private void SeedTestData()
    {
        var category = new BlogCategory
        {
            Id = 1,
            Name = "Test Category",
            Slug = "test-category",
            Description = "Test Description",
            Color = "#000000",
            CreatedAt = DateTime.UtcNow
        };

        _context.BlogCategories.Add(category);

        var post = new BlogPost
        {
            Id = 1,
            Title = "Test Blog Post",
            Slug = "test-blog-post",
            Summary = "Test summary",
            Content = "Test content",
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Author = "Test Author",
            CategoryId = 1,
            ViewCount = 0
        };

        _context.BlogPosts.Add(post);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetPostBySlugAsync_ExistingSlug_ReturnsPost()
    {
        // Arrange
        var slug = "test-blog-post";

        // Act
        var result = await _blogService.GetPostBySlugAsync(slug);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(slug, result.Slug);
        Assert.Equal("Test Blog Post", result.Title);
    }

    [Fact]
    public async Task GetPostBySlugAsync_NonExistingSlug_ReturnsNull()
    {
        // Arrange
        var slug = "non-existing-slug";

        // Act
        var result = await _blogService.GetPostBySlugAsync(slug);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPublishedPostsAsync_ReturnsOnlyPublishedPosts()
    {
        // Arrange
        var unpublishedPost = new BlogPost
        {
            Id = 2,
            Title = "Unpublished Post",
            Slug = "unpublished-post",
            Summary = "Test",
            Content = "Test",
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            Author = "Test",
            CategoryId = 1
        };
        _context.BlogPosts.Add(unpublishedPost);
        await _context.SaveChangesAsync();

        // Act
        var result = await _blogService.GetPublishedPostsAsync(1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.All(result.Items, post => Assert.True(post.IsPublished));
    }

    [Fact]
    public async Task CreatePostAsync_ValidPost_CreatesSuccessfully()
    {
        // Arrange
        var newPost = new BlogPost
        {
            Title = "New Test Post",
            Slug = "new-test-post",
            Summary = "New summary",
            Content = "New content",
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            Author = "Test Author",
            CategoryId = 1
        };

        // Act
        var result = await _blogService.CreatePostAsync(newPost);

        // Assert
        Assert.True(result);
        var savedPost = await _blogService.GetPostBySlugAsync("new-test-post");
        Assert.NotNull(savedPost);
        Assert.Equal("New Test Post", savedPost.Title);
    }

    [Fact]
    public async Task IncrementViewCountAsync_ExistingPost_IncrementsViewCount()
    {
        // Arrange
        var postId = 1;
        var initialViewCount = (await _blogService.GetPostByIdAsync(postId))!.ViewCount;

        // Act
        await _blogService.IncrementViewCountAsync(postId);

        // Assert
        var updatedPost = await _blogService.GetPostByIdAsync(postId);
        Assert.NotNull(updatedPost);
        Assert.Equal(initialViewCount + 1, updatedPost.ViewCount);
    }

    [Fact]
    public async Task GetPostsByCategoryAsync_ExistingCategory_ReturnsFilteredPosts()
    {
        // Arrange
        var categorySlug = "test-category";

        // Act
        var result = await _blogService.GetPostsByCategoryAsync(categorySlug, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.All(result.Items, post => Assert.Equal(1, post.CategoryId));
    }

    [Fact]
    public async Task SearchPostsAsync_MatchingQuery_ReturnsResults()
    {
        // Arrange
        var searchQuery = "Test";

        // Act
        var result = await _blogService.SearchPostsAsync(searchQuery, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task DeletePostAsync_ExistingPost_DeletesSuccessfully()
    {
        // Arrange
        var postId = 1;

        // Act
        var result = await _blogService.DeletePostAsync(postId);

        // Assert
        Assert.True(result);
        var deletedPost = await _blogService.GetPostByIdAsync(postId);
        Assert.Null(deletedPost);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
