using Website_Designer_Ghana.Data.Models;

namespace Website_Designer_Ghana.Services.Interfaces;

public interface ICourseService
{
    // Courses
    Task<Course?> GetCourseByIdAsync(int id);
    Task<Course?> GetCourseBySlugAsync(string slug);
    Task<IEnumerable<Course>> GetAllCoursesAsync(bool publishedOnly = true);
    Task<IEnumerable<Course>> GetFeaturedCoursesAsync();
    Task<Course> CreateCourseAsync(Course course);
    Task UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(int id);

    // Lessons
    Task<IEnumerable<CourseLesson>> GetCourseLessonsAsync(int courseId, bool publishedOnly = true);
    Task<CourseLesson?> GetLessonByIdAsync(int id);
    Task<CourseLesson?> GetLessonBySlugAsync(int courseId, string slug);
    Task<CourseLesson> CreateLessonAsync(CourseLesson lesson);
    Task UpdateLessonAsync(CourseLesson lesson);
    Task DeleteLessonAsync(int id);
}
