namespace TL.Shipay.Project.Domain.Enums
{
    public enum ECodeTypeLog
    {
        None = 0,
        BrasilApiCnpjError,
        BrasilApiNotFound,
        BrasilApiCepError,
        BrasilApiExceptionError,
        BrasilApiTimeoutError,
        ViaCepError,
        ViaCepNotFound,
        ViaCepExceptionError,
        BrasilApiFallbackFail,
        ViaCepFallbackFail,
        DataValidateFail,
        DataValidadeSuccess
    }
}
