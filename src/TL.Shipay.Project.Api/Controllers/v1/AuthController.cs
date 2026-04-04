using Microsoft.AspNetCore.Mvc;
using System.Net;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Domain.Models.Requests;

namespace TL.Shipay.Project.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthAppService _authAppService;

        public AuthController(IAuthAppService authAppService)
        {
            _authAppService = authAppService;
        }

        [HttpPost("gerar-token")]
        public async Task<IActionResult> Login([FromHeader] string username, [FromHeader] string password)
        {
            var response = await _authAppService.ValidaCredencial(username, password);

            if (response == HttpStatusCode.BadRequest) return BadRequest("Username e Password são obrigatórios");

            if (response == HttpStatusCode.Unauthorized) return Unauthorized("Credenciais inválidas");

            var token = await _authAppService.GerarToken(username, Guid.NewGuid().ToString());

            if (string.IsNullOrWhiteSpace(token)) return BadRequest("Ocorreu um erro ao gerar o token");

            return Ok(new
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = 3600
            });
        }
    }

}
