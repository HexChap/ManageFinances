using backend.Data;
using backend.DTOs;
using backend.Exceptions;
using backend.Mappings;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

/// <summary>
/// Business logic for managing expense categories.
/// Categories may be user-owned or global (<see cref="Category.UserId"/> = null).
/// Global categories are visible to all users but cannot be modified by them.
/// </summary>
public class CategoryService
{
    private readonly AppDbContext _db;
    private readonly ILogger<CategoryService> _logger;

    /// <param name="db">Database context.</param>
    /// <param name="logger">Logger for write operation audit trail.</param>
    public CategoryService(AppDbContext db, ILogger<CategoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Creates a new user-owned category.</summary>
    /// <param name="request">Category details.</param>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created category as a response DTO.</returns>
    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, int userId, CancellationToken ct = default)
    {
        var category = new Category { Name = request.Name, UserId = userId };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Category {Id} '{Name}' created", category.Id, category.Name);
        return category.ToResponse();
    }

    /// <summary>Returns all categories visible to the user: their own plus global ones (UserId = null).</summary>
    /// <param name="userId">ID of the authenticated user.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task<IReadOnlyList<CategoryResponse>> GetByUserAsync(int userId, CancellationToken ct = default)
    {
        List<Category> categories = await _db.Categories
            .Where(c => c.UserId == userId || c.UserId == null)
            .ToListAsync(ct);
        return categories.Select(c => c.ToResponse()).ToList();
    }

    /// <summary>Updates the name of a user-owned category. Global categories cannot be updated.</summary>
    /// <param name="id">ID of the category to update.</param>
    /// <param name="request">New category details.</param>
    /// <param name="userId">ID of the authenticated user. Must match the category owner.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated category as a response DTO.</returns>
    /// <exception cref="NotFoundException">Thrown when the category does not exist, is global, or belongs to a different user.</exception>
    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request, int userId, CancellationToken ct = default)
    {
        Category category = await _db.Categories.AsTracking().FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct)
            ?? throw new NotFoundException($"Category {id} not found");
        category.Name = request.Name;
        _db.Categories.Update(category);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Category {Id} updated", id);
        return category.ToResponse();
    }

    /// <summary>Deletes a category by ID.</summary>
    /// <param name="id">ID of the category to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="NotFoundException">Thrown when the category does not exist.</exception>
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        Category category = await _db.Categories.FindAsync([id], ct)
            ?? throw new NotFoundException($"Category {id} not found");
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Category {Id} deleted", id);
    }
}
