using TL.Shipay.Project.Domain.Enums;

namespace TL.Shipay.Project.Domain.Utils
{
    public static class LogMessagesExtensions
    {
        public static string ComDetalheOpcional(this string message, string detalhe, string emptyMarker = "sem detalhe")
        => string.IsNullOrWhiteSpace(detalhe) || detalhe == emptyMarker
            ? message
            : $"{message} - {detalhe}";

        public static string TenteNovamenteMaisTardeComSuporte() => "Tente novamente mais tarde ou contate o suporte para maiores informações.";
        public static string TenteNovamenteMaisTarde() => "Tente novamente mais tarde.";
        public static string SemConfiguracaoResiliencia() => "Não foi definido uma configuração de resiliência para chamadas. Não conseguimos prosseguir.";
        public static string InformacoesNaoCoincidem() => "Infelizmente, a cidade ou o logradouro não são coincidem.";
        public static string InformacoesCoincidem() => "Sucesso!! As informações coincidem.";

        public static string Codigo(this ECodeTypeLog key) =>
        key switch
        {
            ECodeTypeLog.BrasilApiCnpjError => "BrasilApiCnpjError",
            ECodeTypeLog.BrasilApiNotFound => "BrasilApiNotFound",
            ECodeTypeLog.BrasilApiCepError => "BrasilApiCepError",
            ECodeTypeLog.BrasilApiExceptionError => "BrasilApiExceptionError",
            ECodeTypeLog.ViaCepError => "ViaCepError",
            ECodeTypeLog.ViaCepNotFound => "ViaCepNotFound",
            ECodeTypeLog.ViaCepExceptionError => "ViaCepExceptionError",
            ECodeTypeLog.BrasilApiFallbackFail => "BrasilApiFallbackFail",
            ECodeTypeLog.ViaCepFallbackFail => "ViaCepFallbackFail",
            ECodeTypeLog.DataValidateFail => "DataValidateFail",
            ECodeTypeLog.DataValidadeSuccess => "DataValidadeSuccess",
            _ => ""
        };

        public static string Texto(this ETitleLog key) =>
        key switch
        {
            ETitleLog.ViaCepErroConsulta => "Erro na consulta ViaCep",
            ETitleLog.BrasilApiErroConsultaCnpj => "Erro na consulta BrasilApi Cnpj",
            ETitleLog.BrasilApiErroConsultaCep => "Erro na consulta BrasilApi Cep",
            ETitleLog.BrasilApiErroFallback => "Erro na consulta BrasilApi Cnpj",
            ETitleLog.ViaCepErroFallback => "Erro na consulta BrasilApi Cep",
            ETitleLog.ValidacaoDadosFalha => "Informações não foram validadas com sucesso.",
            ETitleLog.ValidacaoDadosSucesso => "Informações validadas com sucesso",
            _ => ""
        };
    }
}
