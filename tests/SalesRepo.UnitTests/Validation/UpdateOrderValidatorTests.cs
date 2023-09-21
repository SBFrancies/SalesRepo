using SalesRepo.Data.Enums;
using SalesRepo.Domain.Models.Request;
using Xunit;

namespace SalesRepo.UnitTests.Validation
{
    public class UpdateOrderValidatorTests
    {
        [Theory]
        [InlineData(OrderStatus.Returned)]
        [InlineData(OrderStatus.Delivered)]
        [InlineData(OrderStatus.Cancelled)]
        [InlineData(OrderStatus.Shipped)]
        public void UpdateOrderValidator_Validate_ValidRequestIsValidated(OrderStatus orderStatus)
        {
            var request = new UpdateOrderRequest
            {
                Status = orderStatus,
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.True(result.IsValid);
        }

       [Fact]
        public void UpdateOrderValidator_Validate_StatusCannotBeUpdatedToPending()
        {
            var request = new UpdateOrderRequest
            {
                Status = OrderStatus.Pending,
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        public UpdateOrderValidator UpdateSystemUnderTest()
        {
            return new UpdateOrderValidator();
        }
    }
}
