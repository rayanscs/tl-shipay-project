using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Application.Interfaces
{
    public interface IClienteService
    {
        Task<bool> ValidaMatchEnderecos(Response enderecoBrasilApi, Response enderecoViaCep);
    }
}
