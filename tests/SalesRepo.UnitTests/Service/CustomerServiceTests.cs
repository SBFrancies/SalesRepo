using FluentValidation;
using Moq;
using SalesRepo.Data.Models;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Service;
using Moq.EntityFrameworkCore;
using Xunit;
using SalesRepo.UnitTests.Validation;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Azure.Core;

namespace SalesRepo.UnitTests.Service
{
    public class CustomerServiceTests
    {
        private readonly Mock<IValidator<CreateCustomerRequest>> _mockCreateCustomerValidator = new();
        private readonly Mock<IValidator<UpdateCustomerRequest>> _mockUpdateCustomerValidator = new();
        private readonly Fixture _fixture = new();

        public CustomerServiceTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                        .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task CustomerService_CreateCustomerAsync_CanCreateValidCustomer()
        {
            var context = new Mock<SalesRepoContext>();
            var customers = new List<Customer>();
            context.Setup(a => a.Customers).ReturnsDbSet(customers);
            context.Setup(a => a.Customers.AddAsync(It.IsAny<Customer>(), default)).Callback<Customer, CancellationToken>((a, _) => customers.Add(a));

            var request = new CreateCustomerRequest
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "Test",
                Phone = "0123456789",
            };

            var sut = GetSystemUnderTest(context.Object);

            var result = await sut.CreateCustomerAsync(request);

            _mockCreateCustomerValidator.Verify(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request)), Times.Once);

            context.Verify(a => a.Customers.AddAsync(It.Is<Customer>(
                 a => a.FirstName == request.FirstName &&
                 a.LastName == request.LastName &&
                 a.Email == request.Email &&
                 a.Phone == request.Phone), default), Times.Once);
            context.Verify(a => a.SaveChangesAsync(default), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.FirstName, result.FirstName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.Phone, result.Phone);

            var customer = customers.Single();

            Assert.NotNull(customer);
            Assert.Equal(request.Email, customer.Email);
            Assert.Equal(request.FirstName, customer.FirstName);
            Assert.Equal(request.LastName, customer.LastName);
            Assert.Equal(request.Phone, customer.Phone);
        }

        [Fact]
        public async Task CustomerService_CreateCustomerAsync_InvalidCustomerThrowsException()
        {
            var context = new Mock<SalesRepoContext>();
            var customers = new List<Customer>();
            context.Setup(a => a.Customers).ReturnsDbSet(customers);
            context.Setup(a => a.Customers.AddAsync(It.IsAny<Customer>(), default)).Callback<Customer, CancellationToken>((a, _) => customers.Add(a));

            var request = new CreateCustomerRequest();
            _mockCreateCustomerValidator.Setup(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request))).Callback(() => new CreateCustomerValidator().ValidateAndThrow(request));

            var sut = GetSystemUnderTest(context.Object);

            await Assert.ThrowsAsync<ValidationException>(async () => await sut.CreateCustomerAsync(request));

            _mockCreateCustomerValidator.Verify(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request)), Times.Once);

            context.Verify(a => a.Customers.AddAsync(It.Is<Customer>(
                 a => a.FirstName == request.FirstName &&
                 a.LastName == request.LastName &&
                 a.Email == request.Email &&
                 a.Phone == request.Phone), default), Times.Never);
            context.Verify(a => a.SaveChangesAsync(default), Times.Never);

            Assert.Empty(customers);
        }

        private CustomerService GetSystemUnderTest(SalesRepoContext? context = null)
        {
            return new CustomerService(() => context ?? Mock.Of<SalesRepoContext>(), _mockCreateCustomerValidator.Object, _mockUpdateCustomerValidator.Object);
        }
    }
}
