using AutoFixture;
using FluentValidation;
using Moq;
using Moq.EntityFrameworkCore;
using SalesRepo.Data.Models;
using SalesRepo.Domain.Exceptions;
using SalesRepo.Domain.Models.Request;
using SalesRepo.Domain.Service;
using SalesRepo.UnitTests.Validation;
using Xunit;

namespace SalesRepo.UnitTests.Service
{
    public class ProductServiceTests
    {
        private readonly Mock<IValidator<CreateProductRequest>> _mockCreateProductValidator = new();
        private readonly Mock<IValidator<UpdateProductRequest>> _mockUpdateProductValidator = new();
        private readonly Fixture _fixture = new();

        public ProductServiceTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                        .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task ProductService_CreateProductAsync_CanCreateValidProduct()
        {
            var context = new Mock<SalesRepoContext>();
            var products = new List<Product>();
            context.Setup(a => a.Products).ReturnsDbSet(products);
            context.Setup(a => a.Products.AddAsync(It.IsAny<Product>(), default)).Callback<Product, CancellationToken>((a, _) => products.Add(a));

            var request = new CreateProductRequest
            {
                Description = "test-description",
                Name = "test-name",
                Sku = "test-sku",
            };

            var sut = GetSystemUnderTest(context.Object);

            var result = await sut.CreateProductAsync(request);

            _mockCreateProductValidator.Verify(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request)), Times.Once);

            context.Verify(a => a.Products.AddAsync(It.Is<Product>(
                 a => a.Sku == request.Sku &&
                 a.Name == request.Name &&
                 a.Description == request.Description), default), Times.Once);
            context.Verify(a => a.SaveChangesAsync(default), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Sku, result.Sku);
            Assert.Equal(request.Name, result.Name);


            var product = products.Single();

            Assert.NotNull(product);
            Assert.Equal(request.Description, product.Description);
            Assert.Equal(request.Sku, product.Sku);
            Assert.Equal(request.Name, product.Name);
        }

        [Fact]
        public async Task ProductService_CreateProductAsync_InvalidProductThrowsException()
        {
            var context = new Mock<SalesRepoContext>();
            var products = new List<Product>();
            context.Setup(a => a.Products).ReturnsDbSet(products);
            context.Setup(a => a.Products.AddAsync(It.IsAny<Product>(), default)).Callback<Product, CancellationToken>((a, _) => products.Add(a));

            var request = new CreateProductRequest();
            _mockCreateProductValidator.Setup(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request))).Callback(() => new CreateProductValidator().ValidateAndThrow(request));

            var sut = GetSystemUnderTest(context.Object);

            await Assert.ThrowsAsync<ValidationException>(async () => await sut.CreateProductAsync(request));

            _mockCreateProductValidator.Verify(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request)), Times.Once);

            context.Verify(a => a.Products.AddAsync(It.Is<Product>(
                 a => a.Sku == request.Sku &&
                 a.Name == request.Name &&
                 a.Description == request.Description), default), Times.Never);
            context.Verify(a => a.SaveChangesAsync(default), Times.Never);

            Assert.Empty(products);
        }

        [Fact]
        public async Task ProductService_GetProductAsync_CanReturnProductById()
        {
            var context = new Mock<SalesRepoContext>();
            var products = TestData.Products;
            context.Setup(a => a.Products).ReturnsDbSet(products);

            var sut = GetSystemUnderTest(context.Object);

            var result = await sut.GetProductAsync(1);
            var existingProduct = products.First(a => a.Id == 1);

            Assert.NotNull(result);
            Assert.Equal(existingProduct.Description, result.Description);
            Assert.Equal(existingProduct.Sku, result.Sku);
            Assert.Equal(existingProduct.Name, result.Name);
            Assert.Equal(existingProduct.Id, result.Id);
        }

        [Fact]
        public async Task ProductService_GetProductAsync_NoMatchingProductThrowsException()
        {
            var context = new Mock<SalesRepoContext>();
            var products = new List<Product>();
            context.Setup(a => a.Products).ReturnsDbSet(products);

            var sut = GetSystemUnderTest(context.Object);

            await Assert.ThrowsAsync<EntityNotFoundException<Product>>(async () => await sut.GetProductAsync(1));
        }

        [Fact]
        public async Task ProductService_GetProductAsync_CanReturnProductList()
        {
            var context = new Mock<SalesRepoContext>();
            var products = TestData.Products;
            context.Setup(a => a.Products).ReturnsDbSet(products);

            var sut = GetSystemUnderTest(context.Object);

            var result = await sut.GetProductListAsync(null);

            Assert.NotEmpty(result);
            Assert.Equal(products.Count, result.Count);
        }

        [Fact]
        public async Task ProductService_GetProductAsync_CanReturnFilteredProductList()
        {
            var context = new Mock<SalesRepoContext>();
            var products = TestData.Products;
            context.Setup(a => a.Products).ReturnsDbSet(products);

            var sut = GetSystemUnderTest(context.Object);

            var result = await sut.GetProductListAsync("test1");

            Assert.NotEmpty(result);
            Assert.Equal(products.Count(a => a.Name.Contains("test1") || a.Sku.Contains("test1")), result.Count);
        }

        [Fact]
        public async Task ProductService_GetProductOrderListAsync_CanReturnProductOrderList()
        {
            var context = new Mock<SalesRepoContext>();
            var products = TestData.Products.Take(1).ToList();
            var orders = TestData.Orders.Where(a => a.ProductId == products[0].Id).ToList();
            foreach (var order in orders)
            {
                order.Product = products[0];
                order.Customer = TestData.Customers.First(a => a.Id == order.CustomerId);
            }
            products[0].Orders = orders;
            context.Setup(a => a.Products).ReturnsDbSet(products);
            context.Setup(a => a.Orders).ReturnsDbSet(orders);

            var sut = GetSystemUnderTest(context.Object);

            var result = await sut.GetProductOrderListAsync(products[0].Id);

            Assert.NotEmpty(result);
            Assert.Equal(products[0].Orders.Count, result.Count);
        }

        [Fact]
        public async Task ProductService_UpdateProductAsync_CanUpdateValidProduct()
        {
            var context = new Mock<SalesRepoContext>();
            var products = TestData.Products;
            context.Setup(a => a.Products).ReturnsDbSet(products);
            context.Setup(a => a.Products.AddAsync(It.IsAny<Product>(), default)).Callback<Product, CancellationToken>((a, _) => products.Add(a));

            var request = new UpdateProductRequest
            {
                Description = "test-description",
                Name = "test-name",
                Sku = "test-sku",
                Id = 1,
            };

            var sut = GetSystemUnderTest(context.Object);

            var result = await sut.UpdateProductAsync(request);

            _mockUpdateProductValidator.Verify(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request)), Times.Once);

            context.Verify(a => a.SaveChangesAsync(default), Times.Once);

            Assert.NotNull(result);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Sku, result.Sku);
            Assert.Equal(request.Name, result.Name);

            var product = products.First(a => a.Id == 1);

            Assert.NotNull(product);
            Assert.Equal(request.Description, product.Description);
            Assert.Equal(request.Sku, product.Sku);
            Assert.Equal(request.Name, product.Name);
        }

        [Fact]
        public async Task ProductService_UpdateProductAsync_InvalidProductThrowsException()
        {
            var context = new Mock<SalesRepoContext>();
            var products = TestData.Products;
            context.Setup(a => a.Products).ReturnsDbSet(products);
            context.Setup(a => a.Products.AddAsync(It.IsAny<Product>(), default)).Callback<Product, CancellationToken>((a, _) => products.Add(a));

            var request = new UpdateProductRequest { Id = 1 };
            _mockUpdateProductValidator.Setup(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request))).Callback(() => new UpdateProductValidator().ValidateAndThrow(request));

            var sut = GetSystemUnderTest(context.Object);

            await Assert.ThrowsAsync<ValidationException>(async () => await sut.UpdateProductAsync(request));

            _mockUpdateProductValidator.Verify(a => a.Validate(It.Is<IValidationContext>(a => a.InstanceToValidate == request)), Times.Once);

            context.Verify(a => a.Products.AddAsync(It.Is<Product>(
                 a => a.Sku == request.Sku &&
                 a.Name == request.Name &&
                 a.Description == request.Description), default), Times.Never);
            context.Verify(a => a.SaveChangesAsync(default), Times.Never);
        }

        private ProductService GetSystemUnderTest(SalesRepoContext? context = null)
        {
            return new ProductService(() => context ?? Mock.Of<SalesRepoContext>(), _mockCreateProductValidator.Object, _mockUpdateProductValidator.Object);
        }
    }
}
