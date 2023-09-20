using FluentValidation;
using Microsoft.Extensions.Internal;
using SalesRepo.Domain.Models.Request;

namespace SalesRepo.UnitTests.Validation;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(a => a.Email).NotNull().NotEmpty().MaximumLength(255).EmailAddress();
        RuleFor(a => a.Phone).NotNull().NotEmpty().MaximumLength(40);
        RuleFor(a => a.FirstName).NotNull().NotEmpty().MaximumLength(100);
        RuleFor(a => a.LastName).NotNull().NotEmpty().MaximumLength(100);
    }
}
