using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Website_Designer_Ghana.Data.Models;

namespace Website_Designer_Ghana.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // Blog Entities
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }

        // Contact
        public DbSet<ContactSubmission> ContactSubmissions { get; set; }

        // Course Entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseLesson> CourseLessons { get; set; }

        // Portfolio Entities
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PortfolioCategory> PortfolioCategories { get; set; }

        // Testimonials
        public DbSet<Testimonial> Testimonials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // BlogPost Configuration
            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.IsPublished);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Posts)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // BlogCategory Configuration
            modelBuilder.Entity<BlogCategory>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // BlogComment Configuration
            modelBuilder.Entity<BlogComment>(entity =>
            {
                entity.HasIndex(e => e.BlogPostId);
                entity.HasIndex(e => e.IsApproved);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(e => e.BlogPost)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(e => e.BlogPostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ParentComment)
                    .WithMany(c => c.Replies)
                    .HasForeignKey(e => e.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // BlogTag Configuration
            modelBuilder.Entity<BlogTag>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // ContactSubmission Configuration
            modelBuilder.Entity<ContactSubmission>(entity =>
            {
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.SubmittedAt);
                entity.HasIndex(e => e.IsRead);
            });

            // Course Configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.IsPublished);
                entity.HasIndex(e => e.IsFeatured);

                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.DiscountPrice).HasPrecision(18, 2);
            });

            // CourseLesson Configuration
            modelBuilder.Entity<CourseLesson>(entity =>
            {
                entity.HasIndex(e => e.CourseId);
                entity.HasIndex(e => e.OrderIndex);

                entity.HasOne(e => e.Course)
                    .WithMany(c => c.Lessons)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Portfolio Configuration
            modelBuilder.Entity<Portfolio>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.IsPublished);
                entity.HasIndex(e => e.IsFeatured);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Projects)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // PortfolioCategory Configuration
            modelBuilder.Entity<PortfolioCategory>(entity =>
            {
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            // Testimonial Configuration
            modelBuilder.Entity<Testimonial>(entity =>
            {
                entity.HasIndex(e => e.IsPublished);
                entity.HasIndex(e => e.IsFeatured);
                entity.HasIndex(e => e.OrderIndex);
            });
        }
    }
}
