using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Domain.Interfaces.ApiManager
{
    public interface IViaCepManager
    {
        Task<Response> ObterEnderecoViaCepAsync(string cep, CancellationToken cancellationToken); 
    }
}
