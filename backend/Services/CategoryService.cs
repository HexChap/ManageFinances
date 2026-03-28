using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CategoryService
{
    private readonly AppDbContext _db;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(AppDbContext db, ILogger<CategoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var category = new Category { Name = request.Name, UserId = request.UserId };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Category {Id} '{Name}' created", category.Id, category.Name);
        return category.ToResponse();
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        List<Category> categories = await _db.Categories
            .Where(c => c.UserId == userId || c.UserId == null)
            .ToListAsync(ct);
        return categories.Select(c => c.ToResponse()).ToList();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Category category = await _db.Categories.FindAsync([id], ct)
            ?? throw new NotFoundException($"Category {id} not found");
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Category {Id} deleted", id);
    }
}
