using System.Net;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Domain.Interfaces.Services;

namespace TL.Shipay.Project.Api.AppService.v1
{
    public class AuthAppService(IAuthService _authService) : IAuthAppService
    {
        public async Task<string> GerarToken(string username, string userId) => await _authService.GerarToken(username, userId);
        public async Task<HttpStatusCode> ValidaCredencial(string username, string password) => await _authService.ValidaCredencial(username, password);
    }
}
