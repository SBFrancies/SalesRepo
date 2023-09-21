using SalesRepo.Domain.Models.Request;
using Xunit;

namespace SalesRepo.UnitTests.Validation
{
    public class CreateProductValidatorTests
    {
        [Fact]
        public void CreateProductValidator_Validate_ValidRequestIsValidated()
        {
            var request = new CreateProductRequest
            {
                Sku = "1234W",
                Description = "This is a description",
                Name = "Name",
            };

            var sut = CreateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        public void CreateProductValidator_Validate_InvalidSkuIsNotValid(string sku)
        {
            var request = new CreateProductRequest
            {
                Sku = sku,
                Description = "This is a description",
                Name = "Name",
            };

            var sut = CreateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        public void CreateProductValidator_Validate_InvalidNameIsNotValid(string name)
        {
            var request = new CreateProductRequest
            {
                Sku = "1234W",
                Description = "This is a description",
                Name = name,
            };

            var sut = CreateSystemUnderTest();

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
        public void CreateProductValidator_Validate_InvalidDescriptionIsNotValid(string description)
        {
            var request = new CreateProductRequest
            {
                Sku = "1234W",
                Description = description,
                Name = "Name",
            };

            var sut = CreateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        public CreateProductValidator CreateSystemUnderTest()
        {
            return new CreateProductValidator();
        }
    }
}
