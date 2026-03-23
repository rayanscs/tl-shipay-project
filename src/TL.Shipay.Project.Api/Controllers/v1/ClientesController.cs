using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;

namespace TL.Shipay.Project.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clientes")]
    [ApiController]
    [Produces("application/json")]
    public class ClientesController(ILogger<ClientesController> _logger, IClienteAppService _clienteAppService) : ControllerBase
    {
        

        // POST api/<ClientesController>
        [HttpPost("validacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] ClienteRequest request, CancellationToken cancellationToken)
        {
            var result = await _clienteAppService.ProcessaValidacaoDadosEmpresa(request.Cnpj, request.Cep, cancellationToken);
            if (!result.Sucesso)
                return NotFound(result);
            else
                return Ok();
        }
    }
}
