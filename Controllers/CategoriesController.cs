using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Raceday.Data;
using Raceday.Models;

namespace Raceday.Controllers
{
    /// <summary>
    /// Manages event categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        public CategoriesController(RaceDayDbContext context) => _context = context;

        /// <summary>Retrieves all categories. (Public)</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
            => await _context.Categories.ToListAsync();

        /// <summary>Retrieves a specific category. (Public)</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return NotFound();
            return c;
        }

        /// <summary>Creates a new category. (Organiser only)</summary>
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryID }, category);
        }

        /// <summary>Updates an existing category. (Organiser only)</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.CategoryID) return BadRequest();
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return NotFound();
            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        /// <summary>Deletes a category. (Organiser only)</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var c = await _context.Categories.FindAsync(id);
            if (c == null) return NotFound();
            _context.Categories.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}