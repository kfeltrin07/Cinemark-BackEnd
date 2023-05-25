using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Filmovi_projekt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Filmovi_projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly BookmarkContext _context;

        public BookmarkController(BookmarkContext context)
        {
            _context = context;
        }

        // GET: api/Bookmarks
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bookmark>>> GetBookmarks()
        {
          if (_context.Bookmark == null)
          {
              return NotFound();
          }
            return await _context.Bookmark.ToListAsync();
        }

        // GET: api/Bookmarks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bookmark>> GetBookmark(int id)
        {
          if (_context.Bookmark == null)
          {
              return NotFound();
          }
            var bookmark = await _context.Bookmark.FindAsync(id);

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
          if (_context.Bookmark == null)
          {
              return Problem("Entity set 'BookmarkContext.Bookmarks'  is null.");
          }
            _context.Bookmark.Add(bookmark);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookmark", new { id = bookmark.id_Bookmark }, bookmark);
        }

        // DELETE: api/Bookmarks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            if (_context.Bookmark == null)
            {
                return NotFound();
            }
            var bookmark = await _context.Bookmark.FindAsync(id);
            if (bookmark == null)
            {
                return NotFound();
            }

            _context.Bookmark.Remove(bookmark);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookmarkExists(int id)
        {
            return (_context.Bookmark?.Any(e => e.id_Bookmark == id)).GetValueOrDefault();
        }

        // GET: api/Bookmark/find
        [Authorize]
        [HttpPost("find")]
        public async Task<IActionResult> Authenticate([FromBody] Bookmark bookmark)
        {
            if (bookmark == null)
                return BadRequest();
         
            var selected = await _context.Bookmark.FirstOrDefaultAsync(x => x.id_user == bookmark.id_user && x.id_film == bookmark.id_film);
            if (selected != null)
                {
                _context.Bookmark.Remove(selected);
                await _context.SaveChangesAsync();
                return NotFound(false);
            }
            return Ok();
            
            

        }
        // GET: api/Bookmarks/ByUser
        [Authorize]
        [HttpOptions("ByUser")]
        public async Task<ActionResult<Bookmark>> ByUser([FromBody] Bookmark bookmark)
        {
            if (bookmark == null)
            {
                return NotFound();
            }
            var selected = await _context.Bookmark.FindAsync(bookmark.id_user);

            if (selected == null)
            {
                return NotFound();
            }

            return selected;
        }
        // GET: api/Bookmark/check
        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] Bookmark bookmark)
        {
            if (bookmark == null)
                return BadRequest();

            var selected = await _context.Bookmark.FirstOrDefaultAsync(x => x.id_user == bookmark.id_user && x.id_film == bookmark.id_film);
            if (selected == null)
            {
                
                return NotFound(false);
            }
            return Ok(true);
        }


        [HttpPost("DeleteBookmarksOfUser/{id}")]
        public async Task<IActionResult> DeleteBookmarksOfUser(int id)
        {
            if (_context.Bookmark == null)
            {
                return NotFound();
            }
            var bookmarks = await _context.Bookmark.Where(x => x.id_user == id).ToListAsync();
            if (bookmarks.IsNullOrEmpty())
            {
                return NotFound();
            }

            _context.Bookmark.RemoveRange(bookmarks);
            _context.SaveChanges();
            await _context.SaveChangesAsync();

            return Ok("Sucess");
        }
    }
}
