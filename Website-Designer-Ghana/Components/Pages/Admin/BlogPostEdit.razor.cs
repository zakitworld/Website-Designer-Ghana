using Microsoft.AspNetCore.Components;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class BlogPostEdit
{
    [Parameter] public int? Id { get; set; }
    [Inject] private IBlogService BlogService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [SupplyParameterFromForm]
    private BlogPostFormModel model { get; set; } = new();
    private IEnumerable<BlogCategory>? categories;
    private bool isSaving = false;

    private void HandleImageUploaded(string filePath)
    {
        model.FeaturedImage = filePath;
    }

    protected override async Task OnInitializedAsync()
    {
        categories = await BlogService.GetAllCategoriesAsync();

        if (Id.HasValue)
        {
            var post = await BlogService.GetPostByIdAsync(Id.Value);
            if (post != null)
            {
                model = new BlogPostFormModel
                {
                    Title = post.Title,
                    Slug = post.Slug,
                    Summary = post.Summary,
                    Content = post.Content,
                    FeaturedImage = post.FeaturedImage,
                    Author = post.Author,
                    CategoryId = post.CategoryId,
                    IsPublished = post.IsPublished,
                    MetaTitle = post.MetaTitle,
                    MetaDescription = post.MetaDescription,
                    MetaKeywords = post.MetaKeywords
                };
            }
        }
        else
        {
            model.Author = "Website Designer Ghana Team";
        }
    }

    private async Task HandleSubmit()
    {
        isSaving = true;
        try
        {
            if (Id.HasValue)
            {
                var post = await BlogService.GetPostByIdAsync(Id.Value);
                if (post != null)
                {
                    post.Title = model.Title;
                    post.Slug = model.Slug;
                    post.Summary = model.Summary;
                    post.Content = model.Content;
                    post.FeaturedImage = model.FeaturedImage;
                    post.Author = model.Author;
                    post.CategoryId = model.CategoryId;
                    post.IsPublished = model.IsPublished;
                    post.MetaTitle = model.MetaTitle;
                    post.MetaDescription = model.MetaDescription;
                    post.MetaKeywords = model.MetaKeywords;
                    post.UpdatedAt = DateTime.UtcNow;

                    if (model.IsPublished && !post.PublishedAt.HasValue)
                    {
                        post.PublishedAt = DateTime.UtcNow;
                    }

                    await BlogService.UpdatePostAsync(post);
                }
            }
            else
            {
                var post = new Website_Designer_Ghana.Data.Models.BlogPost
                {
                    Title = model.Title,
                    Slug = model.Slug,
                    Summary = model.Summary,
                    Content = model.Content,
                    FeaturedImage = model.FeaturedImage,
                    Author = model.Author,
                    CategoryId = model.CategoryId,
                    IsPublished = model.IsPublished,
                    MetaTitle = model.MetaTitle,
                    MetaDescription = model.MetaDescription,
                    MetaKeywords = model.MetaKeywords,
                    CreatedAt = DateTime.UtcNow,
                    PublishedAt = model.IsPublished ? DateTime.UtcNow : null,
                    ViewCount = 0
                };

                await BlogService.CreatePostAsync(post);
            }

            NavigationManager.NavigateTo("/admin/blog-posts");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving blog post: {ex.Message}");
        }
        finally
        {
            isSaving = false;
        }
    }

    private void Cancel()
    {
        NavigationManager.NavigateTo("/admin/blog-posts");
    }

    public class BlogPostFormModel
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Slug is required")]
        [MaxLength(250)]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Summary is required")]
        public string Summary { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        public string? FeaturedImage { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; } = string.Empty;

        public int? CategoryId { get; set; }

        public bool IsPublished { get; set; }

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
    }
}
