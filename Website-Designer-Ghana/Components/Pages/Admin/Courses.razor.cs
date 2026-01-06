using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Services.Interfaces;
using Website_Designer_Ghana.Models.Admin;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class Courses
{
    [Inject] private ICourseService CourseService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private IEnumerable<Website_Designer_Ghana.Data.Models.Course>? courses;
    private Website_Designer_Ghana.Data.Models.Course? editingCourse;
    private bool showModal = false;
    private bool showSuccessMessage = false;
    private string successMessage = string.Empty;
    private bool showErrorMessage = false;
    private string errorMessage = string.Empty;
    private bool isSaving = false;

    private CourseFormModel model = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDataAsync();
            StateHasChanged();
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            courses = await CourseService.GetAllCoursesAsync(publishedOnly: false);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading courses: {ex.Message}");
            courses = new List<Website_Designer_Ghana.Data.Models.Course>();
        }
    }

    private async Task ShowCreateModal()
    {
        editingCourse = null;
        model = new CourseFormModel();
        showModal = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task EditCourse(Website_Designer_Ghana.Data.Models.Course course)
    {
        editingCourse = course;
        model = new CourseFormModel
        {
            Title = course.Title,
            Slug = course.Slug,
            Description = course.Description,
            FeaturedImage = course.FeaturedImage,
            Icon = course.Icon,
            Price = course.Price,
            DiscountPrice = course.DiscountPrice,
            Duration = course.Duration,
            Level = course.Level,
            IsPublished = course.IsPublished
        };
        showModal = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task HandleSubmit()
    {
        isSaving = true;
        showErrorMessage = false;

        try
        {
            if (editingCourse != null)
            {
                editingCourse.Title = model.Title;
                editingCourse.Slug = model.Slug;
                editingCourse.Description = model.Description;
                editingCourse.FeaturedImage = model.FeaturedImage;
                editingCourse.Icon = model.Icon;
                editingCourse.Price = model.Price;
                editingCourse.DiscountPrice = model.DiscountPrice;
                editingCourse.Duration = model.Duration;
                editingCourse.Level = model.Level;
                editingCourse.IsPublished = model.IsPublished;
                editingCourse.UpdatedAt = DateTime.UtcNow;

                if (model.IsPublished && !editingCourse.PublishedAt.HasValue)
                {
                    editingCourse.PublishedAt = DateTime.UtcNow;
                }

                await CourseService.UpdateCourseAsync(editingCourse);
                successMessage = "Course updated successfully.";
            }
            else
            {
                var course = new Website_Designer_Ghana.Data.Models.Course
                {
                    Title = model.Title,
                    Slug = model.Slug,
                    Description = model.Description,
                    FeaturedImage = model.FeaturedImage,
                    Icon = model.Icon,
                    Price = model.Price,
                    DiscountPrice = model.DiscountPrice,
                    Duration = model.Duration,
                    Level = model.Level,
                    IsPublished = model.IsPublished,
                    CreatedAt = DateTime.UtcNow,
                    PublishedAt = model.IsPublished ? DateTime.UtcNow : null
                };

                await CourseService.CreateCourseAsync(course);
                successMessage = "Course created successfully.";
            }

            showSuccessMessage = true;
            await CloseModal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            errorMessage = $"{ex.Message}";
            showErrorMessage = true;
        }
        finally
        {
            isSaving = false;
        }
    }

    private async Task DeleteCourse(int courseId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this course?"))
        {
            try
            {
                await CourseService.DeleteCourseAsync(courseId);
                successMessage = "Course deleted successfully.";
                showSuccessMessage = true;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting course: {ex.Message}");
            }
        }
    }

    private async Task CloseModal()
    {
        showModal = false;
        editingCourse = null;
        await InvokeAsync(StateHasChanged);
    }
}
