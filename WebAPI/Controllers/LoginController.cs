using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPI.Models;
namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        LicenseExamDBContext context = new LicenseExamDBContext();
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        private Account AuthenticateAccount(Account account)
        {
            Account _account = null;
			var data = context.Accounts.FirstOrDefault(a => a.Username == account.Username && a.Password == account.Password);

            return data;
        }

        private string GenerateToken(Account account)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], null,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(Account account)
        {
            Console.WriteLine("Da vao duoc day");
            IActionResult response = Unauthorized();
            var acc = AuthenticateAccount(account);
            if (acc != null)
            {
                var token = GenerateToken(acc);
                response = Ok(new { token = token, acc = acc });
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(Account account)
        {

            context.Accounts.Add(account);
            context.SaveChanges();
            return Ok(account);
        }


    }
}
