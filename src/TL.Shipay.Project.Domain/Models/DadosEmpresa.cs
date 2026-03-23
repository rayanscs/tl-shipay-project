using Newtonsoft.Json;
using TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj;

namespace TL.Shipay.Project.Domain.Models
{
    public class DadosEmpresa
    {
        public string? Cnpj { get; set; }
        public string? Cep { get; set; }
        public string? Municipio { get; set; }
        public string? Logradouro { get; set; }
        public string? RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
    }
}
