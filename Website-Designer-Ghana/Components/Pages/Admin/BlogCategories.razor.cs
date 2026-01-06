using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Services.Interfaces;
using Website_Designer_Ghana.Models.Admin;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class BlogCategories
{
    [Inject] private IBlogService BlogService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private IEnumerable<Website_Designer_Ghana.Data.Models.BlogCategory>? categories;
    private Website_Designer_Ghana.Data.Models.BlogCategory? editingCategory;
    private bool showModal = false;
    private bool showSuccessMessage = false;
    private string successMessage = string.Empty;
    private bool showErrorMessage = false;
    private string errorMessage = string.Empty;
    private bool isSaving = false;

    private CategoryFormModel model = new();

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
            categories = await BlogService.GetAllCategoriesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories: {ex.Message}");
            categories = new List<Website_Designer_Ghana.Data.Models.BlogCategory>();
        }
    }

    private async Task ShowCreateModal()
    {
        editingCategory = null;
        model = new CategoryFormModel { Color = "#3b82f6" };
        showModal = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task EditCategory(Website_Designer_Ghana.Data.Models.BlogCategory category)
    {
        editingCategory = category;
        model = new CategoryFormModel
        {
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            Color = category.Color ?? "#3b82f6"
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
            if (editingCategory != null)
            {
                editingCategory.Name = model.Name;
                editingCategory.Slug = model.Slug;
                editingCategory.Description = model.Description;
                editingCategory.Color = model.Color;

                await BlogService.UpdateCategoryAsync(editingCategory);
                successMessage = "Category updated successfully.";
            }
            else
            {
                var category = new Website_Designer_Ghana.Data.Models.BlogCategory
                {
                    Name = model.Name,
                    Slug = model.Slug,
                    Description = model.Description,
                    Color = model.Color,
                    CreatedAt = DateTime.UtcNow
                };

                await BlogService.CreateCategoryAsync(category);
                successMessage = "Category created successfully.";
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

    private async Task DeleteCategory(int categoryId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this category?"))
        {
            try
            {
                await BlogService.DeleteCategoryAsync(categoryId);
                successMessage = "Category deleted successfully.";
                showSuccessMessage = true;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
            }
        }
    }

    private async Task CloseModal()
    {
        showModal = false;
        editingCategory = null;
        await InvokeAsync(StateHasChanged);
    }
}
