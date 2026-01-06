using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Services.Interfaces;
using Website_Designer_Ghana.Models.Admin;

namespace Website_Designer_Ghana.Components.Pages.Admin;

public partial class Portfolios
{
    [Inject] private IPortfolioService PortfolioService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private IEnumerable<Website_Designer_Ghana.Data.Models.Portfolio>? portfolios;
    private IEnumerable<Website_Designer_Ghana.Data.Models.PortfolioCategory>? categories;
    private Website_Designer_Ghana.Data.Models.Portfolio? editingPortfolio;
    private bool showModal = false;
    private bool showSuccessMessage = false;
    private string successMessage = string.Empty;
    private bool showErrorMessage = false;
    private string errorMessage = string.Empty;
    private bool isSaving = false;

    private PortfolioFormModel model = new();

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
            portfolios = await PortfolioService.GetAllPortfoliosAsync(publishedOnly: false);
            categories = await PortfolioService.GetAllCategoriesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading portfolios: {ex.Message}");
            portfolios = new List<Website_Designer_Ghana.Data.Models.Portfolio>();
        }
    }

    private async Task ShowCreateModal()
    {
        editingPortfolio = null;
        model = new PortfolioFormModel();
        showModal = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task EditPortfolio(Website_Designer_Ghana.Data.Models.Portfolio portfolio)
    {
        editingPortfolio = portfolio;
        model = new PortfolioFormModel
        {
            Title = portfolio.Title,
            Slug = portfolio.Slug,
            Description = portfolio.Description,
            FeaturedImage = portfolio.FeaturedImage,
            ClientName = portfolio.ClientName,
            ProjectUrl = portfolio.ProjectUrl,
            Technologies = portfolio.Technologies,
            CategoryId = portfolio.CategoryId,
            IsPublished = portfolio.IsPublished
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
            if (editingPortfolio != null)
            {
                editingPortfolio.Title = model.Title;
                editingPortfolio.Slug = model.Slug;
                editingPortfolio.Description = model.Description;
                editingPortfolio.FeaturedImage = model.FeaturedImage;
                editingPortfolio.ClientName = model.ClientName;
                editingPortfolio.ProjectUrl = model.ProjectUrl;
                editingPortfolio.Technologies = model.Technologies;
                editingPortfolio.CategoryId = model.CategoryId;
                editingPortfolio.IsPublished = model.IsPublished;
                editingPortfolio.UpdatedAt = DateTime.UtcNow;

                await PortfolioService.UpdatePortfolioAsync(editingPortfolio);
                successMessage = "Portfolio updated successfully.";
            }
            else
            {
                var portfolio = new Website_Designer_Ghana.Data.Models.Portfolio
                {
                    Title = model.Title,
                    Slug = model.Slug,
                    Description = model.Description,
                    FeaturedImage = model.FeaturedImage,
                    ClientName = model.ClientName,
                    ProjectUrl = model.ProjectUrl,
                    Technologies = model.Technologies,
                    CategoryId = model.CategoryId,
                    IsPublished = model.IsPublished,
                    CreatedAt = DateTime.UtcNow
                };

                await PortfolioService.CreatePortfolioAsync(portfolio);
                successMessage = "Portfolio created successfully.";
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

    private async Task DeletePortfolio(int portfolioId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this portfolio?"))
        {
            try
            {
                await PortfolioService.DeletePortfolioAsync(portfolioId);
                successMessage = "Portfolio deleted successfully.";
                showSuccessMessage = true;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting portfolio: {ex.Message}");
            }
        }
    }

    private async Task CloseModal()
    {
        showModal = false;
        editingPortfolio = null;
        await InvokeAsync(StateHasChanged);
    }
}
