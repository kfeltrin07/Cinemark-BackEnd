using Filmovi_projekt.Helpers;
using Filmovi_projekt.Models;
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
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Filmovi_projekt.Models.Dto;
using Microsoft.Identity.Client;

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

        // GET: api/TestUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> Getlogins()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/TestUsers/5
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

        // PUT: api/TestUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
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
                login.password = PasswordHasher.HashPassword(login.password);
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

        // POST: api/TestUsers
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
                login.role = "user";
                login.token = "string";
                login.RefreshToken = "string";
                login.RefreshTokenExpiryTime = DateTime.Now;

                await _context.Users.AddAsync(login);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetLogin", new { id = login.id_user }, login);
            }
            catch (Exception)
            {

            return BadRequest();

            }
        }

        // DELETE: api/TestUsers/5
        [Authorize]
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

        // GET: api/TestUsers/Login
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User login)
        {
            if (login == null)
                return BadRequest();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.username == login.username);

            if (user == null)
                return NotFound(new { Message = "User not Found" });
            else if (!PasswordHasher.VerifyPassword(login.password, user.password))
                return BadRequest(new { Message = "Password is incorrect" });
            else if (user.verified == false)
                return BadRequest(new { Message = "User is not activated" });
            else
            {

                user.token = CreateJwt(user);
                var newAccessToken = user.token;
                var newRefreshToken = CreateRefreshToken();
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30);
                await _context.SaveChangesAsync();

                return Ok(new TokenApiDto()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
        }

        // GET: api/TestUsers/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User login)
        {
            bool sentMail = false;
            string origin = Request.Headers["Origin"];
            Uri uri = new Uri(origin);

            string pageURL = uri.ToString();

            if (login == null)
                return BadRequest(new { Message = "User info is empty" });

            string activationCode = Guid.NewGuid().ToString("N").Substring(0, 25);
            login.password = PasswordHasher.HashPassword(login.password);
            login.activation_code = activationCode;
            login.role = "user";
            login.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30);

            try
            {
                _context.Users.Add(login);
                await _context.SaveChangesAsync();
            }
            catch { 
                return BadRequest(new { Message = "Conflict in database" });
            }
            sentMail = await SendEmailAsync(login, pageURL);

            if (sentMail)
            {
                return Ok(new { Message = "User registered" });
            }
            else
            {
                _context.Users.Remove(login);
                await _context.SaveChangesAsync();
                return BadRequest(new { Message = "Email doesn't exist" });
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
            catch
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

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryveryverysecret.......");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.role),
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Email, user.email),
                new Claim("id_user",user.id_user.ToString())
            });
            var credentials=new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials
            };
            var token=jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        
        private string CreateRefreshToken()
        {
            var tokenBytes= RandomNumberGenerator.GetBytes(64);
            var refreshToken=Convert.ToBase64String(tokenBytes);

            var tokenInUser = _context.Users
                .Any(a=>a.RefreshToken == refreshToken);
            if (tokenInUser) 
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryveryverysecret.......");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal= tokenHandler.ValidateToken(token,tokenValidationParameters, out securityToken);
            var jwtSecurityToken= securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
                {
                throw new SecurityTokenException("This is Invalid Token");
            }
            return principal;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiDto tokenApiDto)
        {
            if (tokenApiDto is null)
                return BadRequest("Invalid Client Request");
            string acessToken = tokenApiDto.AccessToken;
            string refreshToken= tokenApiDto.RefreshToken;
            var principal = GetPrincipleFromExpiredToken(acessToken);
            var username = principal.Identity.Name;
            var user=await _context.Users.FirstOrDefaultAsync(u=>u.username== username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
    }
}

