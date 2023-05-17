using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Filmovi_projekt.Models;

namespace Filmovi_projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Film_GenreController : ControllerBase
    {
        private readonly Film_GenreContext _context;

        public Film_GenreController(Film_GenreContext context)
        {
            _context = context;
        }

        // GET: api/Film_Genre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Film_Genre>>> GetFilm_Genre()
        {
          if (_context.Film_Genre == null)
          {
              return NotFound();
          }
            return await _context.Film_Genre.ToListAsync();
        }

        // GET: api/Film_Genre/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Film_Genre>> GetFilm_Genre(int id)
        {
          if (_context.Film_Genre == null)
          {
              return NotFound();
          }
            var film_Genre = await _context.Film_Genre.FindAsync(id);

            if (film_Genre == null)
            {
                return NotFound();
            }

            return film_Genre;
        }

        // PUT: api/Film_Genre/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilm_Genre(int id, Film_Genre film_Genre)
        {
            if (id != film_Genre.id_genre)
            {
                return BadRequest();
            }

            _context.Entry(film_Genre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Film_GenreExists(id))
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

        // POST: api/Film_Genre
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Film_Genre>> PostFilm_Genre(Film_Genre newFilmGenre)
        {

          if (newFilmGenre == null)
          {
              return Problem("Entity set!");
          }
            
            _context.Film_Genre.Add(newFilmGenre);
            await _context.SaveChangesAsync();

            return  CreatedAtAction("GetFilm_Genre", new { id = newFilmGenre.id_genre }, newFilmGenre);
        }

        // DELETE: api/Film_Genre/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilm_Genre(int id)
        {
            if (_context.Film_Genre == null)
            {
                return NotFound();
            }
            var film_Genre = await _context.Film_Genre.FindAsync(id);
            if (film_Genre == null)
            {
                return NotFound();
            }

            _context.Film_Genre.Remove(film_Genre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Film_GenreExists(int id)
        {
            return (_context.Film_Genre?.Any(e => e.id_genre == id)).GetValueOrDefault();
        }
    }
}
