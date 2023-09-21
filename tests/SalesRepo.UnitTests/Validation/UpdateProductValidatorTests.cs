using SalesRepo.Domain.Models.Request;
using Xunit;

namespace SalesRepo.UnitTests.Validation
{
    public class UpdateProductValidatorTests
    {
        [Fact]
        public void UpdateProductValidator_Validate_ValidRequestIsValidated()
        {
            var request = new UpdateProductRequest
            {
                Sku = "1234W",
                Description = "This is a description",
                Name = "Name",
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        public void UpdateProductValidator_Validate_InvalidSkuIsNotValid(string sku)
        {
            var request = new UpdateProductRequest
            {
                Sku = sku,
                Description = "This is a description",
                Name = "Name",
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        public void UpdateProductValidator_Validate_InvalidNameIsNotValid(string name)
        {
            var request = new UpdateProductRequest
            {
                Sku = "1234W",
                Description = "This is a description",
                Name = name,
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(@"tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooolooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooolooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooolooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooolooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooolooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooolooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo
                      oooooooooooooooooooooooooooooooooooolooooooooooooooooooooooo
                      ooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        public void UpdateProductValidator_Validate_InvalidDescriptionIsNotValid(string description)
        {
            var request = new UpdateProductRequest
            {
                Sku = "1234W",
                Description = description,
                Name = "Name",
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        public UpdateProductValidator UpdateSystemUnderTest()
        {
            return new UpdateProductValidator();
        }
    }
}