using PgTechnicalAssement.Models;

namespace PgTechnicalAssement.Services
{
    public class TransformStockData(AlphaVantageClient alphaVantageClient)
    {
        private readonly AlphaVantageClient _alphaVantageClient = alphaVantageClient;

        public virtual async Task<List<MyApiOutput>> GetTransformedStockDataAsync(string symbol)
        {
            var stockData = await _alphaVantageClient.GetStockDataAsync(symbol);

            if (stockData == null || stockData.TimeSeries == null || stockData.TimeSeries.Count == 0)
            {
                throw new Exception("No stock data available.");
            }

            var cutOffDate = DateTime.UtcNow.AddDays(-30);

            var filteredData = stockData.TimeSeries
                .Select(x =>
                {
                    var timestamp = DateTime.Parse(x.Key);

                    return new IntervalPoint 
                    {
                        Timestamp = timestamp,
                        High = double.Parse(x.Value.High),
                        Low = double.Parse(x.Value.Low),
                        Volume = long.Parse(x.Value.Volume)
                    };
                })
                .Where(z => z.Timestamp >= cutOffDate);

            var transformedData = filteredData
                .GroupBy(date => date.Timestamp.Date)
                .OrderBy(g => g.Key)
                .Select(g => new MyApiOutput
                {
                    Day = g.Key.ToString("yyyy-MM-dd"),
                    LowAverage = g.Average(x => x.Low),
                    HighAverage = g.Average(x => x.High),
                    Volume = g.Sum(x => x.Volume)
                })
                .ToList();

            return transformedData;
        }
    }
}
