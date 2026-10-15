using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Data;
using Raceday.Models;

namespace Raceday.Controllers
{
    /// <summary>
    /// Manages enrolments nested under events, and participant history.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public EnrolmentsController(RaceDayDbContext context) => _context = context;

        /// <summary>
        /// Retrieves all participants enrolled in an event. (Organiser only)
        /// GET: /api/events/{eventId}/enrolments
        /// </summary>
        [HttpGet("events/{eventId}/enrolments")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventEnrolment>>> GetEventEnrolments(int eventId)
        {
            if (!await _context.Events.AnyAsync(e => e.EventID == eventId)) return NotFound();
            return await _context.EventEnrolments
                .Where(en => en.EventID == eventId)
                .Include(en => en.Participant)
                .ToListAsync();
        }

        /// <summary>
        /// Enrols a participant in an event. (Participant only)
        /// POST: /api/events/{eventId}/enrolments
        /// </summary>
        [HttpPost("events/{eventId}/enrolments")]
        [Authorize(Roles = "Participant")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventEnrolment>> CreateEnrolment(int eventId, EventEnrolment enrolment)
        {
            if (!await _context.Events.AnyAsync(e => e.EventID == eventId)) return NotFound();

            enrolment.EventID = eventId;
            enrolment.EnrolmentDate = DateTime.UtcNow;
            enrolment.Status = "Registered";

            _context.EventEnrolments.Add(enrolment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEventEnrolments), new { eventId }, enrolment);
        }

        /// <summary>
        /// Withdraws a participant from an event. (Participant / Organiser)
        /// DELETE: /api/events/{eventId}/enrolments/{enrolmentId}
        /// </summary>
        [HttpDelete("events/{eventId}/enrolments/{enrolmentId}")]
        [Authorize(Roles = "Participant,Organiser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEnrolment(int eventId, int enrolmentId)
        {
            var en = await _context.EventEnrolments
                .FirstOrDefaultAsync(e => e.EnrolmentID == enrolmentId && e.EventID == eventId);
            if (en == null) return NotFound();
            _context.EventEnrolments.Remove(en);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Retrieves a participant's event enrolment history. (Participant / Organiser)
        /// GET: /api/users/{userId}/enrolments
        /// </summary>
        [HttpGet("users/{userId}/enrolments")]
        [Authorize(Roles = "Participant,Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EventEnrolment>>> GetUserEnrolments(int userId)
        {
            if (!await _context.Users.AnyAsync(u => u.UserID == userId)) return NotFound();
            return await _context.EventEnrolments
                .Where(en => en.ParticipantID == userId)
                .Include(en => en.Event)
                .ToListAsync();
        }
    }
}