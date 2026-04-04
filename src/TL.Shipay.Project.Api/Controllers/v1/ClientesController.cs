using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Api.Examples;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Request;

namespace TL.Shipay.Project.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/clientes")]
    [Produces("application/json")]
    [Authorize]
    public class ClientesController(ILogger<ClientesController> _logger, IClienteAppService _clienteAppService, IValidator<ClienteRequest> _validator) : ControllerBase
    {

        /// <summary>
        /// Efetua a verificação dos dados do cliente.
        /// </summary>
        /// <remarks>
        /// Exemplo de chamada: POST /api/v1/clientes/validacao
        /// </remarks>
        /// <returns>Validação se endereços se coicidem</returns>
        [HttpPost("validacao")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
        [SwaggerRequestExample(typeof(ClienteRequest), typeof(ClienteRequestExample))]
        public async Task<ActionResult<Response>> PostAsync([FromBody] ClienteRequest request, CancellationToken cancellationToken)
        {
            var result = await _clienteAppService.ProcessaValidacaoDadosEmpresaAsync(request.Cnpj, request.Cep, cancellationToken);
            return !result.Sucesso ? NotFound(result) : Ok(result);
        }
    }
}
