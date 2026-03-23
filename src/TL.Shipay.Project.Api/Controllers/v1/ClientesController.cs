using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Request;

namespace TL.Shipay.Project.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/clientes")]
    //[Produces("application/json")]
    public class ClientesController(ILogger<ClientesController> _logger, IClienteAppService _clienteAppService) : ControllerBase
    {
        /// <summary>
        /// Efetua a verificação dos dados do cliente.
        /// </summary>
        /// <remarks>
        /// Exemplo de chamada: POST /api/v1/clientes/validacao
        /// </remarks>
        /// <returns>Validação se endereços se coicidem</returns>
        [HttpPost("validacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(ClienteRequest), typeof(ClienteRequestSwaggerExample))]
        public async Task<ActionResult<Response>> PostAsync([FromBody] ClienteRequest request, CancellationToken cancellationToken)
        {
            var result = await _clienteAppService.ProcessaValidacaoDadosEmpresaAsync(request.Cnpj, request.Cep, cancellationToken);
            if (!result.Sucesso)
                return NotFound(result);
            else
                return Ok(result);
        }
    }
}
