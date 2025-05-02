using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MCPServer.Domain.Entities;
using MCPServer.Domain.Interfaces;
using BCrypt.Net;

namespace MCPServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {
            try
            {
                await _userRepository.GetByEmailAsync(request.Email);
                return BadRequest("Email already exists");
            }
            catch (KeyNotFoundException)
            {
                try
                {
                    await _userRepository.GetByUsernameAsync(request.Username);
                    return BadRequest("Username already exists");
                }
                catch (KeyNotFoundException)
                {
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    var user = new User(request.Username, request.Email, passwordHash);

                    await _userRepository.CreateAsync(user);
                    return Ok(user);
                }
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginRequest request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid credentials");
                }

                var token = GenerateJwtToken(user);
                user.UpdateLastLogin();
                await _userRepository.UpdateAsync(user);

                return Ok(new { token });
            }
            catch (KeyNotFoundException)
            {
                return Unauthorized("Invalid credentials");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}