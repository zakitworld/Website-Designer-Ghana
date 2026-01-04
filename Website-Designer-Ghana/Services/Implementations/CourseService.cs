using Microsoft.EntityFrameworkCore;
using Website_Designer_Ghana.Data;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Data.Repositories;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IRepository<CourseLesson> _lessonRepository;
    private readonly ApplicationDbContext _context;

    public CourseService(
        IRepository<Course> courseRepository,
        IRepository<CourseLesson> lessonRepository,
        ApplicationDbContext context)
    {
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
        _context = context;
    }

    // Courses
    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        return await _context.Courses
            .Include(c => c.Lessons.Where(l => l.IsPublished))
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Course?> GetCourseBySlugAsync(string slug)
    {
        return await _context.Courses
            .Include(c => c.Lessons.Where(l => l.IsPublished))
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync(bool publishedOnly = true)
    {
        var query = _context.Courses.AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(c => c.IsPublished);
        }

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Course>> GetFeaturedCoursesAsync()
    {
        return await _context.Courses
            .Where(c => c.IsPublished && c.IsFeatured)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Course> CreateCourseAsync(Course course)
    {
        course.CreatedAt = DateTime.UtcNow;
        if (course.IsPublished && course.PublishedAt == null)
        {
            course.PublishedAt = DateTime.UtcNow;
        }
        return await _courseRepository.AddAsync(course);
    }

    public async Task UpdateCourseAsync(Course course)
    {
        course.UpdatedAt = DateTime.UtcNow;
        if (course.IsPublished && course.PublishedAt == null)
        {
            course.PublishedAt = DateTime.UtcNow;
        }
        await _courseRepository.UpdateAsync(course);
    }

    public async Task DeleteCourseAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);
        if (course != null)
        {
            await _courseRepository.DeleteAsync(course);
        }
    }

    // Lessons
    public async Task<IEnumerable<CourseLesson>> GetCourseLessonsAsync(int courseId, bool publishedOnly = true)
    {
        var query = _context.CourseLessons
            .Where(l => l.CourseId == courseId)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(l => l.IsPublished);
        }

        return await query.OrderBy(l => l.OrderIndex).ToListAsync();
    }

    public async Task<CourseLesson?> GetLessonByIdAsync(int id)
    {
        return await _context.CourseLessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<CourseLesson?> GetLessonBySlugAsync(int courseId, string slug)
    {
        return await _context.CourseLessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.CourseId == courseId && l.Slug == slug);
    }

    public async Task<CourseLesson> CreateLessonAsync(CourseLesson lesson)
    {
        lesson.CreatedAt = DateTime.UtcNow;
        return await _lessonRepository.AddAsync(lesson);
    }

    public async Task UpdateLessonAsync(CourseLesson lesson)
    {
        lesson.UpdatedAt = DateTime.UtcNow;
        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task DeleteLessonAsync(int id)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id);
        if (lesson != null)
        {
            await _lessonRepository.DeleteAsync(lesson);
        }
    }
}
