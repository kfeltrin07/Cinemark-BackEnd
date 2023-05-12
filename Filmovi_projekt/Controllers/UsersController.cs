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
using System.Data;
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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
            try
            {
                if (_context.Users == null)
                {
                    return Problem("Entity set 'LoginContext.logins'  is null.");
                }
                login.password = PasswordHasher.HashPassword(login.password);

                await _context.Users.AddAsync(login);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetLogin", new { id = login.id_user }, login);
            }
            catch (Exception)
            {

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

            if (!PasswordHasher.VerifyPassword(login.password, user.password))
            {
                return BadRequest(new { Message = "Password is incorrect" });
            }

            return Ok(new
            {
                user.id_user
            });
        }

        // GET: api/Logins/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User login)
        {
            bool sentMail = false;
            string origin = Request.Headers["Origin"];
            Uri uri = new Uri(origin);

            string pageURL = uri.ToString();

            if (login == null)
                return BadRequest("User info is empty");

            string activationCode = Guid.NewGuid().ToString("N").Substring(0, 25);
            login.password = PasswordHasher.HashPassword(login.password);
            login.activation_code = activationCode;
            login.role = 0;

            _context.Users.Add(login);
            await _context.SaveChangesAsync();

            sentMail = await SendEmailAsync(login, pageURL);

            if (sentMail)
            {
                return Ok("User Registered!");
            }
            else
            {
                _context.Users.Remove(login);
                await _context.SaveChangesAsync();
                return BadRequest("Email doesn't exist");
            }

        }

        private async Task<bool> SendEmailAsync(User login, string pageURL)
        {
            bool sentMail = false;
            try
            {
                using (MailMessage mm = new MailMessage("ebaukovac@student.foi.hr", login.email))
                {
                    string pageUrl = pageURL + "login?activate=" + login.activation_code + "&idUser=" + login.id_user;
                    mm.Subject = "Account Activation";
                    string body = "Hello " + login.username.Trim() + ",";
                    body += "<br /><br />Please click the following link to activate your account";
                    body += "<br /><a href=\"" + pageUrl + "\">Activate account</a> ";
                    body += "<br /><br />Thanks";
                    mm.Body = body;
                    mm.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("ebaukovac@student.foi.hr", "myzvfmfyqnhrpasm");
                        smtp.EnableSsl = true;
                        try
                        {
                            await smtp.SendMailAsync(mm);
                            sentMail = true;
                        }
                        catch (SmtpFailedRecipientException ex)
                        {
                            sentMail = false;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                sentMail = false;
            }

            return sentMail;
        }

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateUser(string activationCode, int idUser)
        {
            var user = await _context.Users.FindAsync(idUser);
            if (user == null || user.activation_code != activationCode)
                return BadRequest();
            user.verified = true;
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Activated!"
            });
        }

    }
}

