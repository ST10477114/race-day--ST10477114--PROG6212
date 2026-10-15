namespace Raceday.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public int? FinishPosition { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public decimal? PaceMinPerKm { get; set; }

        public string ResultStatus { get; set; } = "Finished";

        
        public EventEnrolment? EventEnrolment { get; set; }
    }
}
