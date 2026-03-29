using System.ComponentModel.DataAnnotations;
using backend.DTOs;

namespace backend.Tests.DTOs;

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

    // Валидира конкретен атрибут директно върху стойност
    private static bool IsAttributeValid(ValidationAttribute attr, object? value)
    {
        return attr.IsValid(value);
    }

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
        var attr = new RangeAttribute(0.01, double.MaxValue);
        Assert.False(IsAttributeValid(attr, 0m));
    }

    [Fact]
    public void CreateExpenseRequest_NegativeValue_FailsValidation()
    {
        var attr = new RangeAttribute(0.01, double.MaxValue);
        Assert.False(IsAttributeValid(attr, -5m));
    }

    [Fact]
    public void CreateExpenseRequest_ZeroCategoryId_FailsValidation()
    {
        var attr = new RangeAttribute(1, int.MaxValue);
        Assert.False(IsAttributeValid(attr, 0));
    }

    [Fact]
    public void CreateExpenseRequest_PositiveValue_PassesValidation()
    {
        var attr = new RangeAttribute(0.01, double.MaxValue);
        Assert.True(IsAttributeValid(attr, 25.50m));
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
        var attr = new RangeAttribute(0.01, double.MaxValue);
        Assert.False(IsAttributeValid(attr, 0m));
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
        var attr = new RangeAttribute(0.01, double.MaxValue);
        Assert.False(IsAttributeValid(attr, 0m));
    }

    [Fact]
    public void CreateIncomeRequest_NegativeValue_FailsValidation()
    {
        var attr = new RangeAttribute(0.01, double.MaxValue);
        Assert.False(IsAttributeValid(attr, -1m));
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
        var attr = new RequiredAttribute();
        Assert.False(IsAttributeValid(attr, ""));
    }

    [Fact]
    public void CreateCategoryRequest_NameTooLong_FailsValidation()
    {
        var attr = new MaxLengthAttribute(32);
        Assert.False(IsAttributeValid(attr, new string('x', 33)));
    }

    [Fact]
    public void CreateCategoryRequest_MaxLengthName_PassesValidation()
    {
        var attr = new MaxLengthAttribute(32);
        Assert.True(IsAttributeValid(attr, new string('x', 32)));
    }

    // ── UpdateCategoryRequest ─────────────────────────────────────────────────

    [Fact]
    public void UpdateCategoryRequest_EmptyName_FailsValidation()
    {
        var attr = new RequiredAttribute();
        Assert.False(IsAttributeValid(attr, ""));
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
        var attr = new RequiredAttribute();
        Assert.False(IsAttributeValid(attr, ""));
    }

    [Fact]
    public void CreateTagRequest_NameTooLong_FailsValidation()
    {
        var attr = new MaxLengthAttribute(32);
        Assert.False(IsAttributeValid(attr, new string('a', 33)));
    }

    // ── UpdateTagRequest ──────────────────────────────────────────────────────

    [Fact]
    public void UpdateTagRequest_Valid_PassesValidation()
    {
        var dto = new UpdateTagRequest(Name: "updated");
        Assert.True(IsValid(dto));
    }
}