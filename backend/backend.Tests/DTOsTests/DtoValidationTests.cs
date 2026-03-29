using System.ComponentModel.DataAnnotations;
using backend.DTOs;

namespace backend.Tests.DTOs;

/// <summary>
/// Tests that DataAnnotation validation attributes on request DTOs behave correctly.
/// Uses Validator.TryValidateObject — same mechanism ASP.NET model binding uses.
/// </summary>
public class DtoValidationTests
{
    private static IList<ValidationResult> Validate(object dto)
    {
        var ctx = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, ctx, results, validateAllProperties: true);
        return results;
    }

    private static bool IsValid(object dto) => Validate(dto).Count == 0;

    // ── CreateExpenseRequest ──────────────────────────────────────────────────

    [Fact]
    public void CreateExpenseRequest_Valid_PassesValidation()
    {
        var dto = new CreateExpenseRequest(CategoryId: 1, Value: 0.01m);
        Assert.True(IsValid(dto));
    }

    [Fact]
    public void CreateExpenseRequest_ZeroValue_FailsValidation()
    {
        var dto = new CreateExpenseRequest(CategoryId: 1, Value: 0m);
        Assert.False(IsValid(dto));
    }

    [Fact]
    public void CreateExpenseRequest_NegativeValue_FailsValidation()
    {
        var dto = new CreateExpenseRequest(CategoryId: 1, Value: -5m);
        Assert.False(IsValid(dto));
    }

    [Fact]
    public void CreateExpenseRequest_ZeroCategoryId_FailsValidation()
    {
        var dto = new CreateExpenseRequest(CategoryId: 0, Value: 10m);
        Assert.False(IsValid(dto));
    }

    // ── UpdateExpenseRequest ──────────────────────────────────────────────────

    [Fact]
    public void UpdateExpenseRequest_Valid_PassesValidation()
    {
        var dto = new UpdateExpenseRequest(CategoryId: 2, Value: 50m);
        Assert.True(IsValid(dto));
    }

    [Fact]
    public void UpdateExpenseRequest_ZeroValue_FailsValidation()
    {
        var dto = new UpdateExpenseRequest(CategoryId: 1, Value: 0m);
        Assert.False(IsValid(dto));
    }

    // ── CreateIncomeRequest ───────────────────────────────────────────────────

    [Fact]
    public void CreateIncomeRequest_Valid_PassesValidation()
    {
        var dto = new CreateIncomeRequest(Value: 100m);
        Assert.True(IsValid(dto));
    }

    [Fact]
    public void CreateIncomeRequest_ZeroValue_FailsValidation()
    {
        var dto = new CreateIncomeRequest(Value: 0m);
        Assert.False(IsValid(dto));
    }

    [Fact]
    public void CreateIncomeRequest_NegativeValue_FailsValidation()
    {
        var dto = new CreateIncomeRequest(Value: -1m);
        Assert.False(IsValid(dto));
    }

    // ── UpdateIncomeRequest ───────────────────────────────────────────────────

    [Fact]
    public void UpdateIncomeRequest_Valid_PassesValidation()
    {
        var dto = new UpdateIncomeRequest(Value: 200m);
        Assert.True(IsValid(dto));
    }

    // ── CreateCategoryRequest ─────────────────────────────────────────────────

    [Fact]
    public void CreateCategoryRequest_Valid_PassesValidation()
    {
        var dto = new CreateCategoryRequest(Name: "Food");
        Assert.True(IsValid(dto));
    }

    [Fact]
    public void CreateCategoryRequest_EmptyName_FailsValidation()
    {
        var dto = new CreateCategoryRequest(Name: "");
        Assert.False(IsValid(dto));
    }

    [Fact]
    public void CreateCategoryRequest_NameTooLong_FailsValidation()
    {
        var dto = new CreateCategoryRequest(Name: new string('x', 33)); // max 32
        Assert.False(IsValid(dto));
    }

    [Fact]
    public void CreateCategoryRequest_MaxLengthName_PassesValidation()
    {
        var dto = new CreateCategoryRequest(Name: new string('x', 32));
        Assert.True(IsValid(dto));
    }

    // ── UpdateCategoryRequest ─────────────────────────────────────────────────

    [Fact]
    public void UpdateCategoryRequest_EmptyName_FailsValidation()
    {
        var dto = new UpdateCategoryRequest(Name: "");
        Assert.False(IsValid(dto));
    }

    // ── CreateTagRequest ──────────────────────────────────────────────────────

    [Fact]
    public void CreateTagRequest_Valid_PassesValidation()
    {
        var dto = new CreateTagRequest(Name: "essential");
        Assert.True(IsValid(dto));
    }

    [Fact]
    public void CreateTagRequest_EmptyName_FailsValidation()
    {
        var dto = new CreateTagRequest(Name: "");
        Assert.False(IsValid(dto));
    }

    [Fact]
    public void CreateTagRequest_NameTooLong_FailsValidation()
    {
        var dto = new CreateTagRequest(Name: new string('a', 33));
        Assert.False(IsValid(dto));
    }

    // ── UpdateTagRequest ──────────────────────────────────────────────────────

    [Fact]
    public void UpdateTagRequest_Valid_PassesValidation()
    {
        var dto = new UpdateTagRequest(Name: "updated");
        Assert.True(IsValid(dto));
    }
}
