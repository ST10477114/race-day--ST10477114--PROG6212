namespace Raceday.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
