using Microsoft.EntityFrameworkCore;
using Website_Designer_Ghana.Data;
using Website_Designer_Ghana.Data.Models;
using Website_Designer_Ghana.Data.Repositories;
using Website_Designer_Ghana.Services.Interfaces;

namespace Website_Designer_Ghana.Services.Implementations;

public class PortfolioService : IPortfolioService
{
    private readonly IRepository<Portfolio> _portfolioRepository;
    private readonly IRepository<PortfolioCategory> _categoryRepository;
    private readonly ApplicationDbContext _context;

    public PortfolioService(
        IRepository<Portfolio> portfolioRepository,
        IRepository<PortfolioCategory> categoryRepository,
        ApplicationDbContext context)
    {
        _portfolioRepository = portfolioRepository;
        _categoryRepository = categoryRepository;
        _context = context;
    }

    // Portfolio Projects
    public async Task<Portfolio?> GetPortfolioByIdAsync(int id)
    {
        return await _context.Portfolios
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Portfolio?> GetPortfolioBySlugAsync(string slug)
    {
        return await _context.Portfolios
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Portfolio>> GetAllPortfoliosAsync(bool publishedOnly = true)
    {
        var query = _context.Portfolios
            .Include(p => p.Category)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Portfolio>> GetFeaturedPortfoliosAsync()
    {
        return await _context.Portfolios
            .Include(p => p.Category)
            .Where(p => p.IsPublished && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(6)
            .ToListAsync();
    }

    public async Task<IEnumerable<Portfolio>> GetPortfoliosByCategoryAsync(int categoryId, bool publishedOnly = true)
    {
        var query = _context.Portfolios
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .AsQueryable();

        if (publishedOnly)
        {
            query = query.Where(p => p.IsPublished);
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<Portfolio> CreatePortfolioAsync(Portfolio portfolio)
    {
        portfolio.CreatedAt = DateTime.UtcNow;
        return await _portfolioRepository.AddAsync(portfolio);
    }

    public async Task UpdatePortfolioAsync(Portfolio portfolio)
    {
        portfolio.UpdatedAt = DateTime.UtcNow;
        await _portfolioRepository.UpdateAsync(portfolio);
    }

    public async Task DeletePortfolioAsync(int id)
    {
        var portfolio = await _portfolioRepository.GetByIdAsync(id);
        if (portfolio != null)
        {
            await _portfolioRepository.DeleteAsync(portfolio);
        }
    }

    public async Task IncrementViewCountAsync(int portfolioId)
    {
        var portfolio = await _portfolioRepository.GetByIdAsync(portfolioId);
        if (portfolio != null)
        {
            portfolio.ViewCount++;
            await _portfolioRepository.UpdateAsync(portfolio);
        }
    }

    // Portfolio Categories
    public async Task<IEnumerable<PortfolioCategory>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<PortfolioCategory?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<PortfolioCategory?> GetCategoryBySlugAsync(string slug)
    {
        return await _categoryRepository.FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<PortfolioCategory> CreateCategoryAsync(PortfolioCategory category)
    {
        category.CreatedAt = DateTime.UtcNow;
        return await _categoryRepository.AddAsync(category);
    }

    public async Task UpdateCategoryAsync(PortfolioCategory category)
    {
        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category != null)
        {
            await _categoryRepository.DeleteAsync(category);
        }
    }
}
