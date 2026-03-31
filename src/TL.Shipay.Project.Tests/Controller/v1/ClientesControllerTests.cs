using AutoFixture;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TL.Shipay.Project.Api.AppService.v1.Interfaces;
using TL.Shipay.Project.Api.Controllers.v1;
using TL.Shipay.Project.Domain.Models.Http;
using TL.Shipay.Project.Domain.Models.Request;
using Xunit;

namespace TL.Shipay.Project.Tests.Controller.v1
{
    public class ClientesControllerTests
    {
        private readonly ClientesController _controller;
        private readonly Fixture _fixture = new();

        private readonly Mock<IClienteAppService> _clienteAppServiceMock = new();
        private readonly Mock<ILogger<ClientesController>> _loggerMock = new();
        private readonly Mock<IValidator<ClienteRequest>> _validatorMock = new();

        public ClientesControllerTests()
        {
            _controller = new (
                _loggerMock.Object,
                _clienteAppServiceMock.Object,
                _validatorMock.Object
                );
        }

        [Fact]
        public async Task PostAsync_Retorna200ComResponseSucessoAsync()
        {
            var request = _fixture.Create<ClienteRequest>();
            var response = _fixture.Create<Response>();

            _clienteAppServiceMock.Setup(x => x.ProcessaValidacaoDadosEmpresaAsync(request.Cnpj, request.Cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.PostAsync(request, CancellationToken.None);

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Response>(okResult.Value);

            Assert.Equal(response, returnValue);

            _clienteAppServiceMock.Verify(x => x.ProcessaValidacaoDadosEmpresaAsync(
                request.Cnpj,
                request.Cep,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _clienteAppServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Post_Retorna404_QuandoResponseComFalha()
        {
            var request = _fixture.Create<ClienteRequest>();

            var response = new Response();
            response.AddNotification("erro");

            _clienteAppServiceMock.Setup(x => x.ProcessaValidacaoDadosEmpresaAsync(request.Cnpj, request.Cep, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.PostAsync(request, CancellationToken.None);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnValue = Assert.IsType<Response>(notFoundResult.Value);

            Assert.Equal(response, returnValue);

            _clienteAppServiceMock.Verify(x => x.ProcessaValidacaoDadosEmpresaAsync(
                request.Cnpj,
                request.Cep,
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _clienteAppServiceMock.VerifyNoOtherCalls();
        }
    }
}
