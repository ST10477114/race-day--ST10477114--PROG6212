namespace Raceday.Models
{
    public class WeatherInfo
    {
        public int WeatherID { get; set; }

        public int EventID { get; set; }

        public decimal? TemperatureC { get; set; }

        public string? Condition { get; set; }

        public decimal? WindSpeedKmh { get; set; }

        public int? HumidityPercent { get; set; }

        public DateTime? ForecastTime { get; set; }

        
        public Event? Event { get; set; }
    }
}
