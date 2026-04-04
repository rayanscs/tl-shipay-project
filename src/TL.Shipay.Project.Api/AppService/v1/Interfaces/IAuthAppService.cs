using System.Net;

namespace TL.Shipay.Project.Api.AppService.v1.Interfaces
{
    public interface IAuthAppService
    {
        public Task<HttpStatusCode> ValidaCredencial(string username, string password);
        public Task<string> GerarToken(string username, string userId);
    }
}
