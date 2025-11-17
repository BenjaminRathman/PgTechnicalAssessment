using System.Text.Json.Serialization;

namespace PgTechnicalAssement.Models
{
    public class AlphaVantageResponse
    {
        [JsonPropertyName("Time Series (15min)")]
        public Dictionary<string, IntervalDataPoint>? TimeSeries { get; set; }
    }

    public class IntervalDataPoint
    {
        [JsonPropertyName("2. high")]
        public string High { get; set; } = default!;

        [JsonPropertyName("3. low")]
        public string Low { get; set; } = default!;

        [JsonPropertyName("5. volume")]
        public string Volume { get; set; } = default!;
    }
}
