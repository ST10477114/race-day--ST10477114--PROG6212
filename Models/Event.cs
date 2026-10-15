namespace Raceday.Models
{
    public class Event
    {
        public int EventID { get; set; }

        public int OrganiserID { get; set; }

        public int CategoryID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime EventDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public string Location { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }

        public int MaxParticipants { get; set; }

        public string Status { get; set; } = "Open";

        public DateTime CreatedAt { get; set; }

        public User? Organiser { get; set; }

        public Category? Category { get; set; }

        public ICollection<EventEnrolment> EventEnrolments { get; set; }
            = new List<EventEnrolment>();

        public WeatherInfo? WeatherInfo { get; set; }
    }
}
