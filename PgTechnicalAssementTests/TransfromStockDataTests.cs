using Microsoft.Extensions.Configuration;
using PgTechnicalAssement.Models;
using PgTechnicalAssement.Services;
using Moq;


namespace PgTechnicalAssementTests
{
    public class TransfromStockDataTests
    {
        private static IConfiguration FakeConfig()
        {
            var settings = new Dictionary<string, string?>
            {
                ["AlphaVantageApiKey"] = "TEST_KEY"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        [Fact]
        public async Task GroupsIntradayPointsIntoDays_AndComputesAveragesAndVolume()
        {
            var today = DateTime.UtcNow.Date;
            var day1 = today.AddDays(-1);
            var day2 = today.AddDays(-2);

            var response = new AlphaVantageResponse
            {
                TimeSeries = new Dictionary<string, IntervalDataPoint>
                {
                    [day1.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss")] = new IntervalDataPoint
                    {
                        High = "10.0",
                        Low = "5.0",
                        Volume = "100"
                    },
                    [day1.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss")] = new IntervalDataPoint
                    {
                        High = "14.0",
                        Low = "7.0",
                        Volume = "150"
                    },
                    [day2.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss")] = new IntervalDataPoint
                    {
                        High = "20.0",
                        Low = "10.0",
                        Volume = "200"
                    }
                }
            };

            

            var httpClient = new HttpClient();

            var mockClient = new Mock<AlphaVantageClient>(httpClient, FakeConfig());

            mockClient
                .Setup(c => c.GetStockDataAsync(It.IsAny<string>(),
                                           It.IsAny<string>(),
                                           It.IsAny<string>()))
                .ReturnsAsync(response);

            var service = new TransformStockData(mockClient.Object);

            var result = await service.GetTransformedStockDataAsync("MSFT");

            Assert.Equal(2, result.Count);

            var day1Summary = result.Single(r => r.Day == day1.ToString("yyyy-MM-dd"));
            var day2Summary = result.Single(r => r.Day == day2.ToString("yyyy-MM-dd"));

            Assert.Equal(6.0, day1Summary.LowAverage, 5); 
            Assert.Equal(12.0, day1Summary.HighAverage, 5); 
            Assert.Equal(250, day1Summary.Volume);          

            Assert.Equal(10.0, day2Summary.LowAverage, 5);
            Assert.Equal(20.0, day2Summary.HighAverage, 5);
            Assert.Equal(200, day2Summary.Volume);
        }

        [Fact]
        public async Task ThrowsException_WhenAlphaClientReturnsNull()
        {
            var mockClient = new Mock<AlphaVantageClient>(
                new HttpClient(),
                FakeConfig());

            mockClient
                .Setup(c => c.GetStockDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((AlphaVantageResponse?)null);

            var service = new TransformStockData(mockClient.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetTransformedStockDataAsync("MSFT"));

            Assert.Equal("No stock data available.", ex.Message);
        }

        [Fact]
        public async Task ThrowsException_WhenTimeSeriesIsEmpty()
        {
            var emptyResponse = new AlphaVantageResponse
            {
                TimeSeries = new Dictionary<string, IntervalDataPoint>()
            };

            var mockClient = new Mock<AlphaVantageClient>(
                new HttpClient(),
                FakeConfig());

            mockClient
                .Setup(c => c.GetStockDataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(emptyResponse);

            var service = new TransformStockData(mockClient.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                service.GetTransformedStockDataAsync("MSFT"));

            Assert.Equal("No stock data available.", ex.Message);
        }

    }
}