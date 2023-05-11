using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Filmovi_projekt.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Routing;
using Filmovi_projekt.Helpers;

namespace Filmovi_projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // GET: api/Logins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Getlogins()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Logins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetLogin(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var login = await _context.Users.FindAsync(id);

            if (login == null)
            {
                return NotFound();
            }

            return login;
        }

        // PUT: api/Logins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogin(int id, User login)
        {
            if (id != login.id_user)
            {
                return BadRequest();
            }

            _context.Entry(login).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginExists(id))
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

        // POST: api/Logins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostLogin(User login)
        {
            try { 
          if (_context.Users == null)
          {
              return Problem("Entity set 'LoginContext.logins'  is null.");
          }
          login.password=PasswordHasher.HashPassword(login.password);
          login.role = 0;
            await _context.Users.AddAsync(login);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetLogin", new { id = login.id_user }, login);
            }
        catch (Exception) {

            return BadRequest("User name postoji");

            }
          }

        // DELETE: api/Logins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogin(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var login = await _context.Users.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }

            _context.Users.Remove(login);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoginExists(int id)
        {
            return (_context.Users?.Any(e => e.id_user == id)).GetValueOrDefault();
        }

        // GET: api/Logins/Login
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User login)
        {
            if (login == null)
                return BadRequest();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.username == login.username);
            if (user == null)
                return NotFound(new { Message = "User not Found" });

            if(!PasswordHasher.VerifyPassword(login.password, user.password))
            {
                return BadRequest(new { Message = "Password is incorrect" });
            }

            return Ok(new
            {
                user
            }) ;
        }

        // GET: api/Logins/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User login)
        {
            if (login == null)
                return BadRequest();
            await _context.Users.AddAsync(login);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered!"
            });
        }

    }
}
