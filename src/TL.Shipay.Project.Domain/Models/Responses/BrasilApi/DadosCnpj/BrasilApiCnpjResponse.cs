using Newtonsoft.Json;

namespace TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCnpj
{
    public class BrasilApiCnpjResponse
    {
        [JsonProperty("uf")]
        public string? Uf { get; set; }

        [JsonProperty("cep")]
        public string? Cep { get; set; }

        [JsonProperty("cnpj")]
        public string? Cnpj { get; set; }

        [JsonProperty("pais")]
        public string? Pais { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("porte")]
        public string? Porte { get; set; }

        [JsonProperty("bairro")]
        public string? Bairro { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        [JsonProperty("ddd_fax")]
        public string? DddFax { get; set; }

        [JsonProperty("municipio")]
        public string? Municipio { get; set; }

        [JsonProperty("logradouro")]
        public string? Logradouro { get; set; }

        [JsonProperty("cnae_fiscal")]
        public int? CnaeFiscal { get; set; }

        [JsonProperty("codigo_pais")]
        public string? CodigoPais { get; set; }

        [JsonProperty("complemento")]
        public string? Complemento { get; set; }

        [JsonProperty("codigo_porte")]
        public int? CodigoPorte { get; set; }

        [JsonProperty("razao_social")]
        public string? RazaoSocial { get; set; }

        [JsonProperty("nome_fantasia")]
        public string? NomeFantasia { get; set; }

        [JsonProperty("capital_social")]
        public decimal? CapitalSocial { get; set; }

        [JsonProperty("ddd_telefone_1")]
        public string? DddTelefone1 { get; set; }

        [JsonProperty("ddd_telefone_2")]
        public string? DddTelefone2 { get; set; }

        [JsonProperty("opcao_pelo_mei")]
        public bool? OpcaoPeloMei { get; set; }

        [JsonProperty("descricao_porte")]
        public string? DescricaoPorte { get; set; }

        [JsonProperty("codigo_municipio")]
        public int? CodigoMunicipio { get; set; }

        [JsonProperty("natureza_juridica")]
        public string? NaturezaJuridica { get; set; }

        [JsonProperty("situacao_especial")]
        public string? SituacaoEspecial { get; set; }

        [JsonProperty("opcao_pelo_simples")]
        public bool? OpcaoPeloSimples { get; set; }

        [JsonProperty("situacao_cadastral")]
        public int? SituacaoCadastral { get; set; }

        [JsonProperty("data_opcao_pelo_mei")]
        public string? DataOpcaoPeloMei { get; set; }

        [JsonProperty("data_exclusao_do_mei")]
        public string? DataExclusaoDoMei { get; set; }

        [JsonProperty("cnae_fiscal_descricao")]
        public string? CnaeFiscalDescricao { get; set; }

        [JsonProperty("codigo_municipio_ibge")]
        public int? CodigoMunicipioIbge { get; set; }

        [JsonProperty("data_inicio_atividade")]
        public string? DataInicioAtividade { get; set; }

        [JsonProperty("data_situacao_especial")]
        public string? DataSituacaoEspecial { get; set; }

        [JsonProperty("data_opcao_pelo_simples")]
        public string? DataOpcaoPeloSimples { get; set; }

        [JsonProperty("data_situacao_cadastral")]
        public string? DataSituacaoCadastral { get; set; }

        [JsonProperty("nome_cidade_no_exterior")]
        public string? NomeCidadeNoExterior { get; set; }

        [JsonProperty("codigo_natureza_juridica")]
        public int? CodigoNaturezaJuridica { get; set; }

        [JsonProperty("data_exclusao_do_simples")]
        public string? DataExclusaoDoSimples { get; set; }

        [JsonProperty("motivo_situacao_cadastral")]
        public int? MotivoSituacaoCadastral { get; set; }

        [JsonProperty("ente_federativo_responsavel")]
        public string? EnteFederativoResponsavel { get; set; }

        [JsonProperty("identificador_matriz_filial")]
        public int? IdentificadorMatrizFilial { get; set; }

        [JsonProperty("qualificacao_do_responsavel")]
        public int? QualificacaoDoResponsavel { get; set; }

        [JsonProperty("descricao_situacao_cadastral")]
        public string? DescricaoSituacaoCadastral { get; set; }

        [JsonProperty("descricao_tipo_de_logradouro")]
        public string? DescricaoTipoDeLogradouro { get; set; }

        [JsonProperty("descricao_motivo_situacao_cadastral")]
        public string? DescricaoMotivoSituacaoCadastral { get; set; }

        [JsonProperty("descricao_identificador_matriz_filial")]
        public string? DescricaoIdentificadorMatrizFilial { get; set; }

        [JsonProperty("qsa")]
        public IEnumerable<Qsa>? Qsa { get; set; }

        [JsonProperty("cnaes_secundarios")]
        public List<CnaesSecundario>? CnaesSecundarios { get; set; }
    }
}
