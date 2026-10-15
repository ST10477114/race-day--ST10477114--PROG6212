using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Data;
using Raceday.Models;

namespace Raceday.Controllers
{
    /// <summary>
    /// Manages weather info nested under events.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public WeatherController(RaceDayDbContext context) => _context = context;

        /// <summary>
        /// Retrieves weather information for an event. (Public)
        /// GET: /api/events/{eventId}/weather
        /// </summary>
        [HttpGet("events/{eventId}/weather")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WeatherInfo>> GetEventWeather(int eventId)
        {
            var w = await _context.WeatherInfo
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.EventID == eventId);
            if (w == null) return NotFound();
            return w;
        }

        /// <summary>
        /// Adds or updates weather information for an event. (Organiser only)
        /// PUT: /api/events/{eventId}/weather
        /// </summary>
        [HttpPut("events/{eventId}/weather")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpsertWeather(int eventId, WeatherInfo weather)
        {
            if (!await _context.Events.AnyAsync(e => e.EventID == eventId)) return NotFound();

            var existing = await _context.WeatherInfo.FirstOrDefaultAsync(w => w.EventID == eventId);
            if (existing == null)
            {
                weather.EventID = eventId;
                _context.WeatherInfo.Add(weather);
            }
            else
            {
                existing.TemperatureC = weather.TemperatureC;
                existing.Condition = weather.Condition;
                existing.WindSpeedKmh = weather.WindSpeedKmh;
                existing.HumidityPercent = weather.HumidityPercent;
                existing.ForecastTime = weather.ForecastTime;
            }
            await _context.SaveChangesAsync();
            return Ok(weather);
        }
    }
}