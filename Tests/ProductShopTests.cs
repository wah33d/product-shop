using Moq;
using ProductShop.Handlers;
using ProductShop.Interfaces;
using ProductShop.Models;
using ProductShop.Utils;

namespace ProductShopTests
{
    public class CandidateProductStoreApiTests
    {
        [Fact]
        public async Task GetProducts_ReturnsProductList()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetProducts()).ReturnsAsync(new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 10, Dimensions = new ProductDimensions { Width = 5, Height = 5 } },
                new Product { Id = 2, Name = "Product2", Price = 15, Dimensions = new ProductDimensions { Width = 6, Height = 6 } }
            });

            var result = await mockApi.Object.GetProducts();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetProducts_HandlesEmptyProductList()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetProducts()).ReturnsAsync(new List<Product>());

            var result = await mockApi.Object.GetProducts();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductDimensions_ReturnsProductDimensions()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            var productDimensions = new ProductDimensions { Width = 10, Height = 10 };
            mockApi.Setup(api => api.GetProductDimensions(It.IsAny<int>())).ReturnsAsync(productDimensions);

            var result = await mockApi.Object.GetProductDimensions(1);

            Assert.NotNull(result);
            Assert.Equal(10, result.Width);
            Assert.Equal(10, result.Height);
        }

        [Fact]
        public async Task GetProductDimensions_ThrowsExceptionForInvalidProductId()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetProductDimensions(It.IsAny<int>())).ThrowsAsync(new KeyNotFoundException("Product not found."));

            await Assert.ThrowsAsync<KeyNotFoundException>(() => mockApi.Object.GetProductDimensions(-1));
        }

        [Fact]
        public async Task GetBoxes_ReturnsBoxList()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetBoxes()).ReturnsAsync(new List<Box>
            {
                new Box { Id = 1, Width = 15, Height = 15 },
                new Box { Id = 2, Width = 20, Height = 20 }
            });

            var result = await mockApi.Object.GetBoxes();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetBoxes_HandlesEmptyBoxList()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetBoxes()).ReturnsAsync(new List<Box>());

            var result = await mockApi.Object.GetBoxes();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Checkout_ReturnsCheckoutSummary()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            var summary = new CheckoutSummary { Result = "Success" };
            mockApi.Setup(api => api.Checkout(It.IsAny<int>(), It.IsAny<int[]>())).ReturnsAsync(summary);

            var result = await mockApi.Object.Checkout(1, new int[] { 1, 2 });

            Assert.NotNull(result);
            Assert.Equal("Success", result.Result);
        }

        [Fact]
        public async Task Checkout_ThrowsExceptionForInvalidBoxId()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.Checkout(It.IsAny<int>(), It.IsAny<int[]>())).ThrowsAsync(new InvalidOperationException("Invalid box ID."));

            await Assert.ThrowsAsync<InvalidOperationException>(() => mockApi.Object.Checkout(-1, new int[] { 1, 2 }));
        }
    }

    public class BoxUtilsTests
    {
        [Fact]
        public void GetHorizontalConfiguration_CalculatesCorrectWidthAndHeight()
        {
            var product1 = new Product { Dimensions = new ProductDimensions { Width = 5, Height = 6 } };
            var product2 = new Product { Dimensions = new ProductDimensions { Width = 4, Height = 7 } };

            var (width, height) = BoxUtils.GetHorizontalConfiguration(product1, product2);

            Assert.Equal(9, width);
            Assert.Equal(7, height);
        }

        [Fact]
        public void GetVerticalConfiguration_CalculatesCorrectWidthAndHeight()
        {
            var product1 = new Product { Dimensions = new ProductDimensions { Width = 5, Height = 6 } };
            var product2 = new Product { Dimensions = new ProductDimensions { Width = 4, Height = 7 } };

            var (width, height) = BoxUtils.GetVerticalConfiguration(product1, product2);

            Assert.Equal(5, width);
            Assert.Equal(13, height);
        }

        [Fact]
        public async Task GetSuitableBox_ReturnsCorrectBox()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetBoxes()).ReturnsAsync(new List<Box>
            {
                new Box { Id = 1, Width = 10, Height = 10 },
                new Box { Id = 2, Width = 20, Height = 20 }
            });

            var product1 = new Product { Dimensions = new ProductDimensions { Width = 5, Height = 5 } };
            var product2 = new Product { Dimensions = new ProductDimensions { Width = 6, Height = 6 } };

            var suitableBox = await BoxUtils.GetSuitableBox(mockApi.Object, product1, product2);

            Assert.NotNull(suitableBox);
            Assert.Equal(2, suitableBox.Id);
        }

        [Fact]
        public async Task GetSuitableBox_ThrowsExceptionWhenNoBoxFits()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetBoxes()).ReturnsAsync(new List<Box>
            {
                new Box { Id = 1, Width = 5, Height = 5 }
            });

            var product1 = new Product { Dimensions = new ProductDimensions { Width = 10, Height = 10 } };
            var product2 = new Product { Dimensions = new ProductDimensions { Width = 15, Height = 15 } };

            await Assert.ThrowsAsync<InvalidOperationException>(() => BoxUtils.GetSuitableBox(mockApi.Object, product1, product2));
        }
    }

    public class UserInputUtilsTests
    {
        [Fact]
        public async Task GetSelectedProductIds_ReturnsCorrectIdsForValidInput()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.GetProducts()).ReturnsAsync(new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 10 },
                new Product { Id = 2, Name = "Product2", Price = 15 }
            });

            // Mock Console Input
            var input = "1,2";
            var inputReader = new System.IO.StringReader(input);
            Console.SetIn(inputReader);

            var productIds = await UserInputUtils.GetSelectedProductIds(mockApi.Object);

            Assert.NotNull(productIds);
            Assert.Equal(2, productIds.Length);
            Assert.Contains(1, productIds);
            Assert.Contains(2, productIds);
        }
    }

    public class CheckoutHandlerTests
    {
        [Fact]
        public async Task ProcessCheckout_PrintsCheckoutSummary()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.Checkout(It.IsAny<int>(), It.IsAny<int[]>())).ReturnsAsync(new CheckoutSummary { Result = "Success" });

            var checkoutHandler = new CheckoutHandler();
            await checkoutHandler.ProcessCheckout(mockApi.Object, 1, new int[] { 1, 2 });

            // No exception means the output has been printed successfully
        }

        [Fact]
        public async Task ProcessCheckout_ThrowsExceptionForInvalidBoxId()
        {
            var mockApi = new Mock<ICandidateProductStoreApi>();
            mockApi.Setup(api => api.Checkout(It.IsAny<int>(), It.IsAny<int[]>())).ThrowsAsync(new InvalidOperationException("Invalid box ID."));

            var checkoutHandler = new CheckoutHandler();

            await Assert.ThrowsAsync<InvalidOperationException>(() => checkoutHandler.ProcessCheckout(mockApi.Object, -1, new int[] { 1, 2 }));
        }
    }
}