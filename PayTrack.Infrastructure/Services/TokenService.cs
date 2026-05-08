using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PayTrack.Application.Interfaces;
using PayTrack.Domain.Entities;
using PayTrack.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenService(IConfiguration config, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
        }
        public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.FullName),
               new Claim(ClaimTypes.Email, user.Email),
               new Claim("PhoneNumber", user.PhoneNumber ?? string.Empty),
               new Claim("FullName", user.FullName),
                new Claim("role", user.Role)
            };


            if (roles != null && roles.Any())
            {
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(5),
                signingCredentials: creds);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<ApplicationUser?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }


        public async Task<bool> ValidateRefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            if (user == null || user.RefreshToken != refreshToken)
                return false;

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return false;

            return true;
        }
        public async Task<string> GenerateAndSaveRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();
            return refreshToken;
        }


    }
}
