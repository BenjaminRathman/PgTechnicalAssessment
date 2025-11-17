namespace PgTechnicalAssement.Models
{
    public class MyApiOutput
    {
        public string Day { get; set; } = default!;
        public double LowAverage { get; set; }
        public double HighAverage { get; set; }
        public long Volume { get; set; }
    }
}
