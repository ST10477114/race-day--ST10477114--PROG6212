using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Data;
using Raceday.Models;

namespace Raceday.Controllers
{
    /// <summary>
    /// Manages results (nested under events, plus a specific result endpoint).
    /// </summary>
    [Route("api")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public ResultsController(RaceDayDbContext context) => _context = context;

        /// <summary>
        /// Retrieves all results for a specific event. (Public)
        /// GET: /api/events/{eventId}/results
        /// </summary>
        [HttpGet("events/{eventId}/results")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Result>>> GetEventResults(int eventId)
        {
            if (!await _context.Events.AnyAsync(e => e.EventID == eventId)) return NotFound();
            return await _context.Results
                .Include(r => r.EventEnrolment)
                .Where(r => r.EventEnrolment!.EventID == eventId)
                .ToListAsync();
        }

        /// <summary>
        /// Records a participant's result. (Organiser only)
        /// POST: /api/events/{eventId}/results
        /// </summary>
        [HttpPost("events/{eventId}/results")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result>> CreateResult(int eventId, Result result)
        {
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetResult), new { id = result.ResultID }, result);
        }

        /// <summary>
        /// Retrieves a specific result. (Public)
        /// GET: /api/results/{id}
        /// </summary>
        [HttpGet("results/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            var r = await _context.Results
                .Include(x => x.EventEnrolment)
                .FirstOrDefaultAsync(x => x.ResultID == id);
            if (r == null) return NotFound();
            return r;
        }

        /// <summary>
        /// Updates a result. (Organiser only)
        /// PUT: /api/results/{id}
        /// </summary>
        [HttpPut("results/{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateResult(int id, Result result)
        {
            if (id != result.ResultID) return BadRequest();
            var existing = await _context.Results.FindAsync(id);
            if (existing == null) return NotFound();
            _context.Entry(existing).CurrentValues.SetValues(result);
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        /// <summary>
        /// Deletes a result. (Organiser only)
        /// DELETE: /api/results/{id}
        /// </summary>
        [HttpDelete("results/{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var r = await _context.Results.FindAsync(id);
            if (r == null) return NotFound();
            _context.Results.Remove(r);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}