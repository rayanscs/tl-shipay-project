using TL.Shipay.Project.Domain.Enums;

namespace TL.Shipay.Project.Domain.ExtensionsMethods
{
    public static class LogMessagesExtensions
    {
        public static string ComDetalheOpcional(this string message, string detalhe, string emptyMarker = "sem detalhe")
        => string.IsNullOrWhiteSpace(detalhe) || detalhe == emptyMarker
            ? message
            : $"{message} - {detalhe}";

        public static string TenteNovamenteMaisTardeComSuporte() => "tente novamente mais tarde ou contate o suporte para maiores informações.";
        
        public static string TenteNovamenteMaisTarde() => "tente novamente mais tarde.";

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
            _ => ""
        };

        public static string Texto(this ETitleLog key) =>
        key switch
        {
            ETitleLog.ViaCepErroConsulta => "Erro na consulta ViaCep",
            ETitleLog.BrasilApiErroConsultaCnpj => "Erro na consulta BrasilApi Cnpj",
            ETitleLog.BrasilApiErroConsultaCep => "Erro na consulta BrasilApi Cep",
            _ => ""
        };
    }
}
