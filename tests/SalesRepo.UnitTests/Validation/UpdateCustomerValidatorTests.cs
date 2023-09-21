using SalesRepo.Domain.Models.Request;
using Xunit;

namespace SalesRepo.UnitTests.Validation
{
    public class UpdateCustomerValidatorTests
    {
        [Fact]
        public void UpdateCustomerValidator_Validate_ValidRequestIsValidated()
        {
            var request = new UpdateCustomerRequest
            {
                Email = "myemail@test.com",
                FirstName = "TestFirst",
                LastName = "TestLast",
                Phone = "01923123456",
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("not-an-email")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        [InlineData("@.com")]
        public void UpdateCustomerValidator_Validate_InvalidEmailIsNotValid(string email)
        {
            var request = new UpdateCustomerRequest
            {
                Email = email,
                FirstName = "TestFirst",
                LastName = "TestLast",
                Phone = "01923123456",
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        public void UpdateCustomerValidator_Validate_InvalidFirstNameIsNotValid(string firstName)
        {
            var request = new UpdateCustomerRequest
            {
                Email = "myemail@test.com",
                FirstName = firstName,
                LastName = "TestLast",
                Phone = "01923123456",
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("tooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooloooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooong")]
        public void UpdateCustomerValidator_Validate_InvalidLastNameIsNotValid(string lastName)
        {
            var request = new UpdateCustomerRequest
            {
                Email = "myemail@test.com",
                FirstName = "TestFirst",
                LastName = lastName,
                Phone = "01923123456",
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("toolooooooooooooooooooooooooooooooooooooong")]
        public void UpdateCustomerValidator_Validate_InvalidPhoneIsNotValid(string phone)
        {
            var request = new UpdateCustomerRequest
            {
                Email = "myemail@test.com",
                FirstName = "TestFirst",
                LastName = "TestLast",
                Phone = phone,
            };

            var sut = UpdateSystemUnderTest();

            var result = sut.Validate(request);

            Assert.False(result.IsValid);
        }

        public UpdateCustomerValidator UpdateSystemUnderTest()
        {
            return new UpdateCustomerValidator();
        }
    }
}
