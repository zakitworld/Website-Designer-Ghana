using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Website_Designer_Ghana.Data.Models;

namespace Website_Designer_Ghana.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        // Seed Admin Role
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Seed Default Admin User
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@websitedesignerghana.com";
        var adminPassword = configuration["AdminUser:Password"];

        // Only seed admin if password is configured (prevents accidental seeding in production with weak password)
        if (!string.IsNullOrEmpty(adminPassword))
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
        // Seed Blog Categories
        if (!context.BlogCategories.Any())
        {
            var categories = new List<BlogCategory>
            {
                new BlogCategory
                {
                    Name = "Web Development",
                    Slug = "web-development",
                    Description = "Latest trends and tips in web development",
                    Color = "#3b82f6",
                    CreatedAt = DateTime.UtcNow
                },
                new BlogCategory
                {
                    Name = "SEO & Marketing",
                    Slug = "seo-marketing",
                    Description = "Search engine optimization and digital marketing strategies",
                    Color = "#10b981",
                    CreatedAt = DateTime.UtcNow
                },
                new BlogCategory
                {
                    Name = "Design Tips",
                    Slug = "design-tips",
                    Description = "UI/UX design best practices and inspiration",
                    Color = "#f59e0b",
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.BlogCategories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Blog Tags
        if (!context.BlogTags.Any())
        {
            var tags = new List<BlogTag>
            {
                new BlogTag { Name = "Blazor", Slug = "blazor", CreatedAt = DateTime.UtcNow },
                new BlogTag { Name = "ASP.NET Core", Slug = "aspnet-core", CreatedAt = DateTime.UtcNow },
                new BlogTag { Name = "SEO", Slug = "seo", CreatedAt = DateTime.UtcNow },
                new BlogTag { Name = "UI/UX", Slug = "ui-ux", CreatedAt = DateTime.UtcNow },
                new BlogTag { Name = "E-commerce", Slug = "ecommerce", CreatedAt = DateTime.UtcNow },
                new BlogTag { Name = "Mobile", Slug = "mobile", CreatedAt = DateTime.UtcNow }
            };

            context.BlogTags.AddRange(tags);
            await context.SaveChangesAsync();
        }

        // Seed Blog Posts
        if (!context.BlogPosts.Any())
        {
            var webDevCategory = context.BlogCategories.First(c => c.Slug == "web-development");
            var seoCategory = context.BlogCategories.First(c => c.Slug == "seo-marketing");
            var designCategory = context.BlogCategories.First(c => c.Slug == "design-tips");

            var blazorTag = context.BlogTags.First(t => t.Slug == "blazor");
            var seoTag = context.BlogTags.First(t => t.Slug == "seo");
            var uiuxTag = context.BlogTags.First(t => t.Slug == "ui-ux");

            var posts = new List<BlogPost>
            {
                new BlogPost
                {
                    Title = "Why Blazor is the Future of Web Development in Ghana",
                    Slug = "why-blazor-is-future-web-development-ghana",
                    Summary = "Discover why Blazor is becoming the go-to framework for modern web applications and how it's transforming web development in Ghana.",
                    Content = @"
                        <p>Blazor is revolutionizing web development by allowing developers to build interactive web UIs using C# instead of JavaScript. This powerful framework from Microsoft is gaining tremendous popularity in Ghana's tech ecosystem.</p>

                        <h3>What Makes Blazor Special?</h3>
                        <p>Blazor enables full-stack development with C#, eliminating the need to switch between languages. This consistency improves developer productivity and code maintainability.</p>

                        <h3>Benefits for Ghanaian Businesses</h3>
                        <ul>
                            <li>Faster development cycles</li>
                            <li>Reduced development costs</li>
                            <li>Better performance</li>
                            <li>Easier maintenance</li>
                        </ul>

                        <h3>Real-World Applications</h3>
                        <p>Many businesses in Ghana are already leveraging Blazor for their e-commerce platforms, business management systems, and customer portals. The framework's ability to create progressive web apps (PWAs) makes it ideal for the Ghanaian market where mobile internet usage is high.</p>

                        <h3>Getting Started with Blazor</h3>
                        <p>If you're a business owner in Ghana looking to build a modern web application, Blazor should be at the top of your list. Contact Website Designer Ghana to learn how we can help you leverage this powerful technology.</p>
                    ",
                    FeaturedImage = "/images/blog/blazor-future.jpg",
                    IsPublished = true,
                    PublishedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Author = "Website Designer Ghana Team",
                    ViewCount = 245,
                    CategoryId = webDevCategory.Id,
                    MetaTitle = "Why Blazor is the Future of Web Development in Ghana | Website Designer Ghana",
                    MetaDescription = "Learn why Blazor is transforming web development in Ghana and how it can benefit your business with faster development and better performance.",
                    MetaKeywords = "Blazor Ghana, web development Ghana, ASP.NET Core, modern web framework"
                },
                new BlogPost
                {
                    Title = "10 SEO Strategies Every Ghanaian Business Must Implement",
                    Slug = "10-seo-strategies-ghanaian-business-must-implement",
                    Summary = "Boost your online presence with these proven SEO strategies tailored specifically for businesses operating in Ghana.",
                    Content = @"
                        <p>Search Engine Optimization (SEO) is crucial for any business that wants to be found online. Here are 10 proven strategies that every Ghanaian business should implement to improve their search rankings.</p>

                        <h3>1. Local SEO Optimization</h3>
                        <p>Register your business on Google My Business and ensure your NAP (Name, Address, Phone) information is consistent across all platforms. This is especially important for businesses targeting customers in Accra, Kumasi, and other Ghanaian cities.</p>

                        <h3>2. Mobile-First Approach</h3>
                        <p>With over 70% of internet users in Ghana accessing the web via mobile devices, having a mobile-responsive website is non-negotiable.</p>

                        <h3>3. Quality Content Creation</h3>
                        <p>Create content that addresses the specific needs and questions of your Ghanaian audience. Use local examples and references to make your content more relatable.</p>

                        <h3>4. Page Speed Optimization</h3>
                        <p>Given the varying internet speeds in Ghana, optimizing your website for fast loading is critical. Compress images, minify code, and use a CDN.</p>

                        <h3>5. Local Backlinks</h3>
                        <p>Build relationships with other Ghanaian websites and businesses to earn quality backlinks. This improves your domain authority and local relevance.</p>

                        <h3>6-10. Additional Strategies</h3>
                        <ul>
                            <li>Optimize for voice search with conversational keywords</li>
                            <li>Use structured data markup</li>
                            <li>Create location-specific landing pages</li>
                            <li>Leverage social media for local engagement</li>
                            <li>Monitor and respond to online reviews</li>
                        </ul>

                        <p>Implementing these strategies will significantly improve your online visibility in Ghana. Need help with your SEO? Contact Website Designer Ghana today!</p>
                    ",
                    FeaturedImage = "/images/blog/seo-strategies.jpg",
                    IsPublished = true,
                    PublishedAt = DateTime.UtcNow.AddDays(-3),
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Author = "Website Designer Ghana Team",
                    ViewCount = 189,
                    CategoryId = seoCategory.Id,
                    MetaTitle = "10 Essential SEO Strategies for Ghanaian Businesses in 2026",
                    MetaDescription = "Discover the top 10 SEO strategies that Ghanaian businesses must implement to dominate local search results and attract more customers.",
                    MetaKeywords = "SEO Ghana, local SEO, Ghana business marketing, search engine optimization"
                },
                new BlogPost
                {
                    Title = "Modern Web Design Trends in Ghana for 2026",
                    Slug = "modern-web-design-trends-ghana-2026",
                    Summary = "Stay ahead of the curve with the latest web design trends that are shaping the digital landscape in Ghana.",
                    Content = @"
                        <p>The web design landscape in Ghana is evolving rapidly. Here are the top design trends that are dominating in 2026 and how you can incorporate them into your website.</p>

                        <h3>1. Bold Typography</h3>
                        <p>Large, bold fonts are making a statement on Ghanaian websites. This trend improves readability on mobile devices and creates a strong visual hierarchy.</p>

                        <h3>2. Dark Mode Options</h3>
                        <p>More users prefer dark mode for reduced eye strain and better battery life. Offering a dark mode toggle is becoming standard practice.</p>

                        <h3>3. Micro-Animations</h3>
                        <p>Subtle animations enhance user experience without overwhelming visitors. From hover effects to loading animations, these small details make a big difference.</p>

                        <h3>4. Minimalist Design</h3>
                        <p>Clean, uncluttered designs with plenty of white space are trending. This approach loads faster and provides a better user experience on slower connections.</p>

                        <h3>5. Local Cultural Elements</h3>
                        <p>Incorporating Ghanaian colors, patterns, and imagery helps create a connection with local audiences. Kente patterns, Adinkra symbols, and local photography are popular choices.</p>

                        <h3>6. Accessibility First</h3>
                        <p>Designing for accessibility ensures your website can be used by everyone, including people with disabilities. This includes proper color contrast, keyboard navigation, and screen reader compatibility.</p>

                        <h3>Implementation Tips</h3>
                        <p>When implementing these trends, always prioritize user experience over aesthetics. A beautiful design that doesn't function well serves no purpose.</p>

                        <p>Ready to modernize your website? Website Designer Ghana specializes in creating stunning, functional websites that incorporate the latest design trends while maintaining peak performance.</p>
                    ",
                    FeaturedImage = "/images/blog/design-trends-2026.jpg",
                    IsPublished = true,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Author = "Website Designer Ghana Team",
                    ViewCount = 156,
                    CategoryId = designCategory.Id,
                    MetaTitle = "Web Design Trends in Ghana 2026 | Modern Design Tips",
                    MetaDescription = "Explore the latest web design trends shaping Ghana's digital landscape in 2026. Learn how to create modern, user-friendly websites.",
                    MetaKeywords = "web design Ghana, design trends 2026, UI/UX Ghana, modern website design"
                }
            };

            context.BlogPosts.AddRange(posts);
            await context.SaveChangesAsync();

            // Add tags to posts
            var post1 = context.BlogPosts.First(p => p.Slug == "why-blazor-is-future-web-development-ghana");
            post1.Tags.Add(blazorTag);

            var post2 = context.BlogPosts.First(p => p.Slug == "10-seo-strategies-ghanaian-business-must-implement");
            post2.Tags.Add(seoTag);

            var post3 = context.BlogPosts.First(p => p.Slug == "modern-web-design-trends-ghana-2026");
            post3.Tags.Add(uiuxTag);

            await context.SaveChangesAsync();
        }

        // Seed Portfolio Categories
        if (!context.PortfolioCategories.Any())
        {
            var portfolioCategories = new List<PortfolioCategory>
            {
                new PortfolioCategory
                {
                    Name = "E-Commerce",
                    Slug = "ecommerce",
                    Description = "Online stores and shopping platforms",
                    Icon = "bi-cart",
                    CreatedAt = DateTime.UtcNow
                },
                new PortfolioCategory
                {
                    Name = "Corporate",
                    Slug = "corporate",
                    Description = "Business and corporate websites",
                    Icon = "bi-building",
                    CreatedAt = DateTime.UtcNow
                },
                new PortfolioCategory
                {
                    Name = "Portfolio",
                    Slug = "portfolio",
                    Description = "Personal and professional portfolios",
                    Icon = "bi-briefcase",
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.PortfolioCategories.AddRange(portfolioCategories);
            await context.SaveChangesAsync();
        }
    }
}
