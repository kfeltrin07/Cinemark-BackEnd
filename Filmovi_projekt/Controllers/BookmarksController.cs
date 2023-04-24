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
    public class BookmarksController : ControllerBase
    {
        private readonly BookmarkContext _context;

        public BookmarksController(BookmarkContext context)
        {
            _context = context;
        }

        // GET: api/Bookmarks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetBookmarks()
        {
          if (_context.Bookmarks == null)
          {
              return NotFound();
          }
            return await _context.Bookmarks.ToListAsync();
        }

        // GET: api/Bookmarks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookmark>> GetBookmark(int id)
        {
          if (_context.Bookmarks == null)
          {
              return NotFound();
          }
            var bookmark = await _context.Bookmarks.FindAsync(id);

            if (bookmark == null)
            {
                return NotFound();
            }

            return bookmark;
        }

        // PUT: api/Bookmarks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookmark(int id, Bookmark bookmark)
        {
            if (id != bookmark.id_Bookmark)
            {
                return BadRequest();
            }

            _context.Entry(bookmark).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookmarkExists(id))
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

        // POST: api/Bookmarks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bookmark>> PostBookmark(Bookmark bookmark)
        {
          if (_context.Bookmarks == null)
          {
              return Problem("Entity set 'BookmarkContext.Bookmarks'  is null.");
          }
            _context.Bookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookmark", new { id = bookmark.id_Bookmark }, bookmark);
        }

        // DELETE: api/Bookmarks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            if (_context.Bookmarks == null)
            {
                return NotFound();
            }
            var bookmark = await _context.Bookmarks.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookmarkExists(int id)
        {
            return (_context.Bookmarks?.Any(e => e.id_Bookmark == id)).GetValueOrDefault();
        }
    }
}
