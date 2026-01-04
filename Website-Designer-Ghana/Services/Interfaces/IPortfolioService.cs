using Website_Designer_Ghana.Data.Models;

namespace Website_Designer_Ghana.Services.Interfaces;

public interface IPortfolioService
{
    // Portfolio Projects
    Task<Portfolio?> GetPortfolioByIdAsync(int id);
    Task<Portfolio?> GetPortfolioBySlugAsync(string slug);
    Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync(bool publishedOnly = true);
    Task<IEnumerable<Portfolio>> GetFeaturedPortfoliosAsync();
    Task<IEnumerable<Portfolio>> GetPortfoliosByCategoryAsync(int categoryId, bool publishedOnly = true);
    Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio);
    Task UpdatePortfolioAsync(Portfolio portfolio);
    Task DeletePortfolioAsync(int id);
    Task IncrementViewCountAsync(int portfolioId);

    // Portfolio Categories
    Task<IEnumerable<PortfolioCategory>> GetAllCategoriesAsync();
    Task<PortfolioCategory?> GetCategoryByIdAsync(int id);
    Task<PortfolioCategory?> GetCategoryBySlugAsync(string slug);
    Task<PortfolioCategory> CreateCategoryAsync(PortfolioCategory category);
    Task UpdateCategoryAsync(PortfolioCategory category);
    Task DeleteCategoryAsync(int id);
}
