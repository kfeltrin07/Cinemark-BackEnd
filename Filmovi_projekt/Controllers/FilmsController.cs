using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Filmovi_projekt.Models;

namespace Filmovi_projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly FilmsContext _context;

        public FilmsController(FilmsContext context)
        {
            _context = context;
        }

        // GET: api/Films
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Films>>> GetFilmovi()
        {
          if (_context.Films == null)
          {
              return NotFound();
          }
            return await _context.Films.ToListAsync();
        }

        // GET: api/Films/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Films>> GetFilms(int id)
        {
          if (_context.Films == null)
          {
              return NotFound();
          }
            var films = await _context.Films.FindAsync(id);

            if (films == null)
            {
                return NotFound();
            }

            return films;
        }

        // PUT: api/Films/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilms(int id, Films films)
        {
            if (id != films.id_film)
            {
                return BadRequest();
            }

            _context.Entry(films).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Films
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Films>> PostFilms(Films films)
        {
          if (_context.Films == null)
          {
              return Problem("Entity set 'FilmsContext.Filmovi'  is null.");
          }
            _context.Films.Add(films);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFilms", new { id = films.id_film }, films);
        }

        // DELETE: api/Films/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilms(int id)
        {
            if (_context.Films == null)
            {
                return NotFound();
            }
            var films = await _context.Films.FindAsync(id);
            if (films == null)
            {
                return NotFound();
            }

            _context.Films.Remove(films);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilmsExists(int id)
        {
            return (_context.Films?.Any(e => e.id_film == id)).GetValueOrDefault();
        }
    }
}
