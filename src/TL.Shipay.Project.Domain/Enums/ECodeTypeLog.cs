namespace TL.Shipay.Project.Domain.Enums
{
    public enum ECodeTypeLog
    {
        None = 0,
        BrasilApiCnpjError,
        BrasilApiNotFound,
        BrasilApiCepError,
        BrasilApiExceptionError,
        ViaCepError,
        ViaCepNotFound,
        ViaCepExceptionError
    }
}
