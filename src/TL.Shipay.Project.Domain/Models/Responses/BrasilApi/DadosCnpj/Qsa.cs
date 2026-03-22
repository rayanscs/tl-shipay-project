using Newtonsoft.Json;

namespace TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj
{
    public class Qsa
    {
        [JsonProperty("pais")]
        public string? Pais { get; set; }

        [JsonProperty("nome_socio")]
        public string? NomeSocio { get; set; }

        [JsonProperty("codigo_pais")]
        public string? CodigoPais { get; set; }

        [JsonProperty("faixa_etaria")]
        public string? FaixaEtaria { get; set; }

        [JsonProperty("cnpj_cpf_do_socio")]
        public string? CnpjCpfDoSocio { get; set; }

        [JsonProperty("qualificacao_socio")]
        public string? QualificacaoSocio { get; set; }

        [JsonProperty("codigo_faixa_etaria")]
        public int? CodigoFaixaEtaria { get; set; }

        [JsonProperty("data_entrada_sociedade")]
        public string? DataEntradaSociedade { get; set; }

        [JsonProperty("identificador_de_socio")]
        public int? IdentificadorDeSocio { get; set; }

        [JsonProperty("cpf_representante_legal")]
        public string? CpfRepresentanteLegal { get; set; }

        [JsonProperty("nome_representante_legal")]
        public string? NomeRepresentanteLegal { get; set; }

        [JsonProperty("codigo_qualificacao_socio")]
        public int? CodigoQualificacaoSocio { get; set; }

        [JsonProperty("qualificacao_representante_legal")]
        public string? QualificacaoRepresentanteLegal { get; set; }

        [JsonProperty("codigo_qualificacao_representante_legal")]
        public int? CodigoQualificacaoRepresentanteLegal { get; set; }
    }
}
