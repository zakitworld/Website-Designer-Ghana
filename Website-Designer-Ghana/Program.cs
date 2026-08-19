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

using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "ZAK-I.T.-WORLD")
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

// Configure Forwarded Headers for reverse proxies (e.g. Railway, MonsterASP)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.ForwardLimit = 1;
});

// Add Rate Limiting
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    // Global rate limiter with fixed window (reduced from 200 to 100 requests per minute)
    rateLimiterOptions.GlobalLimiter = System.Threading.RateLimiting.PartitionedRateLimiter.Create<Microsoft.AspNetCore.Http.HttpContext, string>(httpContext =>
        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown",
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "__Host-ZakItWorld.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "__Host-ZakItWorld.Antiforgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Configure Database (SQL Server)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Allow an environment-provided connection string to override settings (common in PaaS deployments)
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string finalConnectionString = connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // If an explicit SQL Server style connection string is supplied, use it directly.
    // Also allow a URL form like sqlserver://user:pass@host:port/database
    if (databaseUrl.StartsWith("sqlserver://", StringComparison.OrdinalIgnoreCase) || databaseUrl.StartsWith("mssql://", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            var user = userInfo[0];
            var password = userInfo.Length > 1 ? userInfo[1] : "";
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 1433;
            var database = uri.AbsolutePath.TrimStart('/');
            finalConnectionString = $"Server={host},{port};Initial Catalog={database};User ID={user};Password={password};Encrypt=True;TrustServerCertificate=False;";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to parse DATABASE_URL for SQL Server, falling back to DefaultConnection");
        }
    }
    else
    {
        // If the environment variable already contains a provider-style connection string, use it.
        if (databaseUrl.Contains("Server=") || databaseUrl.Contains("Data Source=") || databaseUrl.Contains("Initial Catalog="))
        {
            finalConnectionString = databaseUrl;
        }
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(finalConnectionString));
    
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

builder.Services.AddScoped<IEmailSender<ApplicationUser>, IdentityEmailSender>();

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
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Ensure the EF migrations history table exists before attempting migrations.
        // For SQL Server, create the table if it does not exist using an IF NOT EXISTS check.
        await context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
            BEGIN
                CREATE TABLE [__EFMigrationsHistory] (
                    [MigrationId] nvarchar(150) NOT NULL PRIMARY KEY,
                    [ProductVersion] nvarchar(32) NOT NULL
                );
            END
        ");

        await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        await DatabaseSeeder.SeedAsync(context, userManager, roleManager, configuration);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
app.UseForwardedHeaders();

// Only use response compression in production to avoid dev tool issues
if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Append("Content-Security-Policy",
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' cdn.jsdelivr.net; " +
        "style-src 'self' 'unsafe-inline' fonts.googleapis.com cdn.jsdelivr.net; " +
        "font-src 'self' fonts.gstatic.com cdn.jsdelivr.net; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' wss:; " +
        "object-src 'none'; base-uri 'self'; form-action 'self'; frame-ancestors 'none'; upgrade-insecure-requests;");
    context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=(), payment=(), usb=()");
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
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

app.UseStaticFiles(); // Fallback for embedded resources if MapStaticAssets fails
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Map health checks endpoint
app.MapHealthChecks("/health").RequireAuthorization(policy => policy.RequireRole("Admin"));

// Map sitemap.xml endpoint
app.MapGet("/sitemap.xml", async (ISitemapService sitemapService) =>
{
    var sitemap = await sitemapService.GenerateSitemapAsync();
    return Results.Content(sitemap, "application/xml");
}).CacheOutput("static-pages");

app.Run();
