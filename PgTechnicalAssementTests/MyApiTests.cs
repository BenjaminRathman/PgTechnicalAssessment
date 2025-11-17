
using Microsoft.AspNetCore.Mvc;
using Moq;
using PgTechnicalAssement.Controllers;
using PgTechnicalAssement.Models;
using PgTechnicalAssement.Services;


namespace PgTechnicalAssesmentTests
{
    public class MyApiControllerTests
    {
        [Fact]
        public async Task GetStockData_ReturnsBadRequest_WhenSymbolIsMissing()
        {
            var mockService = new Mock<TransformStockData>((AlphaVantageClient)null!);
            var controller = new MyApi(mockService.Object);

            var result = await controller.GetStockData(" ");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

            var body = badRequest.Value!;
            var messageProp = body.GetType().GetProperty("message");
            Assert.NotNull(messageProp);

            var message = messageProp!.GetValue(body) as string;
            Assert.Equal("Symbol parameter is required.", message);
        }

        [Fact]
        public async Task GetStockData_ReturnsNotFound_WhenNoDataReturned()
        {
            var mockService = new Mock<TransformStockData>((AlphaVantageClient)null!);
            mockService
                .Setup(s => s.GetTransformedStockDataAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<MyApiOutput>());

            var controller = new MyApi(mockService.Object);

            var result = await controller.GetStockData("QQQ");

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);

            var value = notFound.Value!;
            var messageProp = value.GetType().GetProperty("message");
            Assert.NotNull(messageProp);

            var message = messageProp.GetValue(value) as string;
            Assert.NotNull(message);
            Assert.Contains("No stock data found for 'QQQ'.", message);
        }

        [Fact]
        public async Task GetStockData_ReturnsOk_WithTransformedData()
        {
            var sample = new List<MyApiOutput>
            {
                new MyApiOutput
                {
                    Day = "2025-01-10",
                    LowAverage = 10.5,
                    HighAverage = 12.3,
                    Volume = 123456
                }
            };

            var mockService = new Mock<TransformStockData>((AlphaVantageClient)null!);
            mockService
                .Setup(s => s.GetTransformedStockDataAsync("QQQ"))
                .ReturnsAsync(sample);

            var controller = new MyApi(mockService.Object);

            var result = await controller.GetStockData("QQQ");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<List<MyApiOutput>>(ok.Value);
            Assert.Single(value);
            Assert.Equal("2025-01-10", value[0].Day);
            Assert.Equal(10.5, value[0].LowAverage);
            Assert.Equal(12.3, value[0].HighAverage);
            Assert.Equal(123456, value[0].Volume);
        }

    }
}

