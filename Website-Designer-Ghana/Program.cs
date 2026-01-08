using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Website_Designer_Ghana.Components;
using Website_Designer_Ghana.Components.Account;
using Website_Designer_Ghana.Data;
using Website_Designer_Ghana.Data.Repositories;
using Website_Designer_Ghana.Services.Interfaces;
using Website_Designer_Ghana.Services.Implementations;
using Website_Designer_Ghana.Services.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Website-Designer-Ghana")
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Rate Limiting
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    // Global rate limiter with fixed window (reduced from 200 to 100 requests per minute)
    rateLimiterOptions.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<Microsoft.AspNetCore.Http.HttpContext, string>(httpContext =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Add output caching for performance
builder.Services.AddOutputCache(options =>
{
    // Default cache policy: 10 minutes
    options.AddBasePolicy(builder => builder
        .Expire(TimeSpan.FromMinutes(10))
        .Tag("default"));

    // Blog posts cache: 1 hour
    options.AddPolicy("blog-posts", builder => builder
        .Expire(TimeSpan.FromHours(1))
        .Tag("blog")
        .SetVaryByQuery("page", "category", "tag"));

    // Static pages cache: 30 minutes
    options.AddPolicy("static-pages", builder => builder
        .Expire(TimeSpan.FromMinutes(30))
        .Tag("static"));

    // Portfolio cache: 1 hour
    options.AddPolicy("portfolio", builder => builder
        .Expire(TimeSpan.FromHours(1))
        .Tag("portfolio"));

    // Course catalog cache: 1 hour
    options.AddPolicy("courses", builder => builder
        .Expire(TimeSpan.FromHours(1))
        .Tag("courses"));
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        // Require email confirmation in production for security
        options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment();
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;

        // Password requirements
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register Generic Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Business Services
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileUploadService, LocalFileUploadService>();
builder.Services.AddScoped<ISitemapService, SitemapService>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        await DatabaseSeeder.SeedAsync(context, userManager, roleManager, configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
// Only use response compression in production to avoid dev tool issues
if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Enable output caching
app.UseOutputCache();

// Enable rate limiting
app.UseRateLimiter();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Map health checks endpoint
app.MapHealthChecks("/health");

// Map sitemap.xml endpoint
app.MapGet("/sitemap.xml", async (ISitemapService sitemapService) =>
{
    var sitemap = await sitemapService.GenerateSitemapAsync();
    return Results.Content(sitemap, "application/xml");
}).CacheOutput("static-pages");

app.Run();
