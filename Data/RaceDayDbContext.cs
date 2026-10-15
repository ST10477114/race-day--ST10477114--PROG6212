using Raceday.Models;
using Microsoft.EntityFrameworkCore;
namespace Raceday.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<WeatherInfo> WeatherInfo { get; set; }

        public DbSet<EventEnrolment> EventEnrolments { get; set; }

        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Category
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryID);

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            // Event
            modelBuilder.Entity<Event>()
                .HasKey(e => e.EventID);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany()
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // EventEnrolment
            modelBuilder.Entity<EventEnrolment>()
                .HasKey(e => e.EnrolmentID);

            modelBuilder.Entity<EventEnrolment>()
                .HasOne(e => e.Event)
                .WithMany(e => e.EventEnrolments)
                .HasForeignKey(e => e.EventID);

            modelBuilder.Entity<EventEnrolment>()
                .HasOne(e => e.Participant)
                .WithMany()
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventEnrolment>()
                .HasIndex(e => new { e.EventID, e.ParticipantID })
                .IsUnique();

            // Result - one enrolment can have at most one result
            modelBuilder.Entity<Result>()
                .HasKey(r => r.ResultID);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.EventEnrolment)
                .WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrolmentID);

            // Weather - one event can have at most one weather record
            modelBuilder.Entity<WeatherInfo>()
                .HasKey(w => w.WeatherID);

            modelBuilder.Entity<WeatherInfo>()
                .HasOne(w => w.Event)
                .WithOne(e => e.WeatherInfo)
                .HasForeignKey<WeatherInfo>(w => w.EventID);
        }
    }
}
