namespace Raceday.Models
{
    public class EventEnrolment
    {
        public int EnrolmentID { get; set; }

        public int EventID { get; set; }

        public int ParticipantID { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string Status { get; set; } = "Registered";

        // Relationships
        public Event? Event { get; set; }

        public User? Participant { get; set; }

        public Result? Result { get; set; }
    }
}
