using System.Text;
using System.Xml.Linq;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Services.Implementations;

public class SitemapService : ISitemapService
{
    private readonly IBlogService _blogService;
    private readonly IPortfolioService _portfolioService;
    private readonly ICourseService _courseService;
    private readonly ILogger<SitemapService> _logger;
    private const string BaseUrl = "https://websitedesignerghana.com";

    public SitemapService(
        IBlogService blogService,
        IPortfolioService portfolioService,
        ICourseService courseService,
        ILogger<SitemapService> logger)
    {
        _blogService = blogService;
        _portfolioService = portfolioService;
        _courseService = courseService;
        _logger = logger;
    }

    public async Task<string> GenerateSitemapAsync()
    {
        try
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(ns + "urlset")
            );

            var urlset = sitemap.Root!;

            // Add static pages
            AddUrl(urlset, ns, "/", "1.0", "daily");
            AddUrl(urlset, ns, "/blog", "0.9", "daily");
            AddUrl(urlset, ns, "/courses", "0.8", "weekly");
            AddUrl(urlset, ns, "/pricing", "0.8", "monthly");
            AddUrl(urlset, ns, "/contact", "0.7", "monthly");

            // Add blog posts
            var blogPosts = await _blogService.GetAllPostsAsync(publishedOnly: true);
            foreach (var post in blogPosts)
            {
                AddUrl(urlset, ns, $"/blog/{post.Slug}", "0.8", "weekly", post.UpdatedAt ?? post.CreatedAt);
            }

            // Add blog categories
            var categories = await _blogService.GetAllCategoriesAsync();
            foreach (var category in categories)
            {
                AddUrl(urlset, ns, $"/blog?category={category.Slug}", "0.7", "weekly");
            }

            // Add portfolio items
            var portfolios = await _portfolioService.GetAllPortfoliosAsync(publishedOnly: true);
            foreach (var portfolio in portfolios)
            {
                AddUrl(urlset, ns, $"/portfolio/{portfolio.Slug}", "0.7", "monthly", portfolio.UpdatedAt ?? portfolio.CreatedAt);
            }

            // Add courses
            var courses = await _courseService.GetAllCoursesAsync(publishedOnly: true);
            foreach (var course in courses)
            {
                AddUrl(urlset, ns, $"/courses/{course.Slug}", "0.8", "weekly", course.UpdatedAt ?? course.CreatedAt);
            }

            return sitemap.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating sitemap");
            throw;
        }
    }

    private void AddUrl(XElement urlset, XNamespace ns, string location, string priority, string changeFrequency, DateTime? lastModified = null)
    {
        var url = new XElement(ns + "url",
            new XElement(ns + "loc", BaseUrl + location),
            new XElement(ns + "priority", priority),
            new XElement(ns + "changefreq", changeFrequency)
        );

        if (lastModified.HasValue)
        {
            url.Add(new XElement(ns + "lastmod", lastModified.Value.ToString("yyyy-MM-dd")));
        }

        urlset.Add(url);
    }
}
