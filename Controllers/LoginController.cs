using backend_project_core.Data;
using backend_project_core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend_project_core.Controllers
{
    [Route("api")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly MyDbContext _userDbContext; //Tạo kết nối với database
        private readonly AppSetting _appSettings; //Cấp key tạo token

        public LoginController(MyDbContext userDbContext, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _userDbContext = userDbContext;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginModel model)
        {
            // Chuyển đổi code thành dạng json
            var enteredPasswordHash = Users.HashPassword(model.password);

            // Kiểm tra xem user có tồn tại trong cơ sở dữ liệu hay không
            var existingUser = _userDbContext.Users.SingleOrDefault(p => p.email == model.email && p.password == enteredPasswordHash);

            if (existingUser == null) //không đúng
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid username/password"
                });
            }

            //cấp token
            var userResponse = new
            {
                Id = existingUser.id,
                Name = existingUser.name,
                Email = existingUser.email
            };
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Authenticate success",
                access_token = GenerateToken(existingUser),
                foundUser = userResponse
            });

        }
        // Hàm tạo token
        private string GenerateToken(Users user)
        {

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.name),
                    new Claim(ClaimTypes.Email, user.email),
                    new Claim("Id", user.id.ToString()),

                    //roles

                    new Claim("TokenId", Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
