using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace TL.Shipay.Project.Api.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clientes")]
    [ApiController]
    [Produces("application/json")]
    public class ClientesController : ControllerBase
    {
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(ILogger<ClientesController> logger)
        {
            _logger = logger;
        }

        // POST api/<ClientesController>
        [HttpPost("validacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] ClienteRequest request)
        {
            return Ok();
        }
    }
}
