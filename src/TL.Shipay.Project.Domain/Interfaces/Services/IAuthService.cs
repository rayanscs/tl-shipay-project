using System.Net;

namespace TL.Shipay.Project.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<HttpStatusCode> ValidaCredencial(string username, string password);
        public Task<string> GerarToken(string username, string userId);
    }
}
