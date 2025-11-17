using PgTechnicalAssement.Models;

namespace PgTechnicalAssement.Services
{
    public class AlphaVantageClient(HttpClient httpClient, IConfiguration configuration)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _apiKey = configuration["AlphaVantageApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "AlphaVantageApiKey is not configured.");

        public virtual async Task<AlphaVantageResponse?> GetStockDataAsync(string symbol, string interval = "15min", string outputSize = "full")
        {
            var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={symbol}&interval={interval}&outputsize={outputSize}&apikey={_apiKey}";
            return await _httpClient.GetFromJsonAsync<AlphaVantageResponse>(url);
        }
    }
}
