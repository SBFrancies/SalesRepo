using FluentValidation;
using SalesRepo.Data.Enums;
using SalesRepo.Domain.Models.Request;

namespace SalesRepo.UnitTests.Validation;

public class UpdateOrderValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderValidator()
    {
        RuleFor(a => a.Status).NotEqual(OrderStatus.Pending).IsInEnum();
    }
}
