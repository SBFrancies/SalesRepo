using FluentValidation;
using SalesRepo.Domain.Models.Request;

namespace SalesRepo.UnitTests.Validation;

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(a => a.Name).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(a => a.Description).NotNull().NotEmpty().MaximumLength(500);
        RuleFor(a => a.Sku).NotNull().NotEmpty().MaximumLength(40);
    }
}