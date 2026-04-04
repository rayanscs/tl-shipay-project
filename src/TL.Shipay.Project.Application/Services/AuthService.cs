using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TL.Shipay.Project.Domain.Interfaces.Services;
using TL.Shipay.Project.Infrastructure;

namespace TL.Shipay.Project.Application.Services
{
    public class AuthService(IOptions<Jwt> _options) : IAuthService
    {
        public async Task<HttpStatusCode> ValidaCredencial(string username, string password)
        {
            return await Task.FromResult((username, password) switch
            {
                (null, _) => HttpStatusCode.BadRequest,
                (_, null) => HttpStatusCode.BadRequest,
                ("svc-intg-xpto", "senha123") => HttpStatusCode.OK,
                _ => HttpStatusCode.Unauthorized
            });     
        }

        public async Task<string> GerarToken(string username, string userId)
        {
            var jwtSettings = _options.Value;
            var key = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(jwtSettings.Key!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresInMinutes!),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult(tokenString);
        }
    }
}
