# Website Designer Ghana

A professional web design agency portfolio and marketing website built with ASP.NET Core 10.0 Blazor Server.

## Overview

Website Designer Ghana is a comprehensive web platform showcasing web design and development services for Ghanaian businesses. The application features a public-facing marketing site with portfolio showcase, blog, course catalog, and a complete admin panel for content management.

## Technology Stack

### Frontend
- **Framework:** Blazor Server (.NET 10.0) with Interactive Server render mode
- **UI Framework:** Bootstrap 5.3.3
- **Icons:** Bootstrap Icons 1.11.3 + Devicon
- **Fonts:** Google Fonts (Inter & Outfit)
- **Styling:** Custom CSS with glassmorphism and dark theme

### Backend
- **Runtime:** .NET 10.0
- **Framework:** ASP.NET Core 10.0
- **Language:** C# with nullable reference types enabled
- **Architecture:** Clean Architecture with Repository Pattern and Service Layer

### Database
- **ORM:** Entity Framework Core 10.0
- **Provider:** SQL Server
- **Development:** LocalDB
- **Production:** SQL Server

### Security & Authentication
- **Identity:** ASP.NET Core Identity
- **Authentication:** Cookie-based authentication
- **Authorization:** Role-based (Admin role)
- **Rate Limiting:** Endpoint-specific limits
- **Security Headers:** X-Content-Type-Options, X-Frame-Options, X-XSS-Protection, Referrer-Policy

### Additional Features
- **Email:** MailKit 4.14.1
- **Logging:** Serilog with file and console sinks
- **Health Checks:** EF Core database health checks
- **Response Compression:** Enabled for HTTPS

## Features

### Public Features
- Home page with services showcase
- Portfolio gallery with categories
- Blog with categories, tags, and comments
- Course catalog
- Contact form with spam protection
- Responsive design (mobile-first)
- SEO-optimized (meta tags, OpenGraph, Twitter cards)

### Admin Features
- Dashboard with statistics
- Blog post management (CRUD)
- Blog category management
- Comment moderation
- Portfolio management
- Course management
- Contact submission inbox
- File upload management

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server) or SQL Server LocalDB
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or VS Code
- [Node.js](https://nodejs.org/) (optional, for frontend build tools)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/Website-Designer-Ghana.git
cd Website-Designer-Ghana
```

### 2. Configure Database

Update the connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-Website_Designer_Ghana-YOUR_GUID;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Configure Admin User

For development, the admin credentials are in `appsettings.Development.json`:

```json
{
  "AdminUser": {
    "Email": "admin@websitedesignerghana.com",
    "Password": "Admin@123456"
  }
}
```

**WARNING:** Change this password immediately in production!

### 4. Apply Database Migrations

```bash
cd Website-Designer-Ghana
dotnet ef database update
```

This will create the database and seed it with:
- Default admin user
- Blog categories and tags
- Sample blog posts
- Portfolio categories

### 5. Run the Application

```bash
dotnet run
```

The application will be available at:
- HTTP: `http://website_designer_ghana.dev.localhost:5038`
- HTTPS: `https://website_designer_ghana.dev.localhost:7170`

### 6. Login to Admin Panel

Navigate to `/Account/Login` and use the admin credentials:
- Email: `admin@websitedesignerghana.com`
- Password: `Admin@123456` (development only)

## Project Structure

```
Website-Designer-Ghana/
├── Components/
│   ├── Account/          # Authentication & user management
│   ├── Layout/           # Layout components (MainLayout, AdminLayout, Navigation)
│   ├── Pages/            # Routable pages
│   │   ├── Admin/        # Admin CRUD pages
│   │   └── [Public pages]
│   └── Shared/           # Reusable components
├── Data/
│   ├── Models/           # Entity models (10 entities)
│   ├── Migrations/       # EF Core migrations
│   └── Repositories/     # Generic repository pattern
├── Services/
│   ├── Interfaces/       # Service contracts
│   ├── Implementations/  # Service implementations
│   └── Models/           # Service DTOs
├── wwwroot/              # Static assets
│   ├── images/
│   ├── js/
│   ├── portfolio_images/
│   └── uploads/
└── Properties/           # Launch settings

## Configuration

### Development Configuration

`appsettings.Development.json`:
- Email confirmation disabled
- Detailed error pages
- Verbose logging
- LocalDB connection

### Production Configuration

`appsettings.Production.json`:
- Email confirmation enabled
- HTTPS enforced
- Production SMTP settings (SendGrid recommended)
- SQL Server connection
- Reduced logging verbosity

**Required Production Environment Variables:**

```bash
# Connection String
ConnectionStrings__DefaultConnection="Server=YOUR_SERVER;Database=YOUR_DB;..."

# Admin User (set strong password!)
AdminUser__Email="admin@websitedesignerghana.com"
AdminUser__Password="YOUR_STRONG_PASSWORD_HERE"

# Email Settings (example for SendGrid)
EmailSettings__SmtpHost="smtp.sendgrid.net"
EmailSettings__SmtpPort="587"
EmailSettings__SmtpUsername="apikey"
EmailSettings__SmtpPassword="YOUR_SENDGRID_API_KEY"
EmailSettings__UseSsl="true"
```

## Security Considerations

### Critical Security Settings

1. **Admin Password:** Never use the default password in production
2. **Email Confirmation:** Enabled in production (disabled in dev)
3. **Rate Limiting:**
   - Global: 100 requests/minute
   - Auth endpoints: 5 attempts/5 minutes
   - Contact form: 3 submissions/hour
4. **File Upload Security:**
   - Magic number validation (file signature checking)
   - File size limits (50MB max)
   - Extension whitelist
   - Sanitized filenames

### Password Requirements (Production)

- Minimum 8 characters
- Requires digit
- Requires uppercase letter
- Requires lowercase letter
- Requires non-alphanumeric character

### Account Lockout

- Max failed attempts: 5
- Lockout duration: 15 minutes
- Enabled for new users

## Deployment

### Prerequisites
1. SQL Server database
2. SMTP service (SendGrid, AWS SES, or similar)
3. Web hosting (Azure App Service, AWS, or IIS)

### Deployment Steps

1. Update `appsettings.Production.json` with production values
2. Build the application:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
3. Upload files to your hosting provider
4. Configure environment variables
5. Run database migrations on production database
6. Test the application

### Azure App Service Deployment

```bash
# Login to Azure
az login

# Create resource group
az group create --name rg-website-designer-ghana --location eastus

# Create app service plan
az appservice plan create --name plan-website-designer-ghana --resource-group rg-website-designer-ghana --sku B1

# Create web app
az webapp create --name website-designer-ghana --resource-group rg-website-designer-ghana --plan plan-website-designer-ghana --runtime "DOTNET|10.0"

# Configure connection string
az webapp config connection-string set --name website-designer-ghana --resource-group rg-website-designer-ghana --connection-string-type SQLServer --settings DefaultConnection="YOUR_CONNECTION_STRING"

# Deploy
az webapp up --name website-designer-ghana --resource-group rg-website-designer-ghana
```

## Development

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName
```

Update database:
```bash
dotnet ef database update
```

Rollback migration:
```bash
dotnet ef database update PreviousMigrationName
```

### Logging

Logs are stored in `logs/` directory with daily rolling files (retained for 30 days).

View logs:
```bash
# Real-time log viewing
tail -f logs/log-YYYYMMDD.txt
```

### Testing

Run tests:
```bash
dotnet test
```

## API Endpoints

### Public Routes
- `/` - Home page
- `/blog` - Blog listing
- `/blog/{slug}` - Individual blog post
- `/courses` - Course catalog
- `/pricing` - Pricing page
- `/contact` - Contact form

### Admin Routes (Requires Authentication)
- `/admin` - Dashboard
- `/admin/blog-posts` - Blog management
- `/admin/blog-categories` - Category management
- `/admin/blog-comments` - Comment moderation
- `/admin/portfolios` - Portfolio management
- `/admin/courses` - Course management
- `/admin/contact-submissions` - Contact inbox

### Health Check
- `/health` - Application health status

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Troubleshooting

### Database Connection Issues

**Problem:** Cannot connect to database

**Solution:**
1. Verify SQL Server is running
2. Check connection string in appsettings.json
3. Ensure database exists (run migrations)
4. Check firewall settings

### Email Not Sending

**Problem:** Contact form emails not being sent

**Solution:**
1. Check SMTP settings in appsettings.json
2. Verify SMTP credentials
3. Check firewall/network restrictions
4. Review logs in `logs/` directory

### Admin Login Not Working

**Problem:** Cannot login to admin panel

**Solution:**
1. Verify admin user was seeded (check database)
2. Ensure correct email/password
3. Check if account is locked out
4. Review authentication logs

## License

This project is proprietary software owned by Website Designer Ghana.

## Support

For support, email support@websitedesignerghana.com or visit our website.

## Acknowledgments

- Built with [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- UI powered by [Bootstrap](https://getbootstrap.com/)
- Icons from [Bootstrap Icons](https://icons.getbootstrap.com/)
- Email via [MailKit](https://github.com/jstedfast/MailKit)
- Logging by [Serilog](https://serilog.net/)
