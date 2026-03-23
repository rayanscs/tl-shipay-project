using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using TL.Shipay.Project.Domain.Models.Request;

namespace TL.Shipay.Project.Api.Examples
{
    [ExcludeFromCodeCoverage]
    public sealed class ClienteRequestExample : IExamplesProvider<ClienteRequest>
    {
        public ClienteRequest GetExamples() => new (
            Cnpj: "123456780001",
            Cep: "12345678"
        );
    }
}
