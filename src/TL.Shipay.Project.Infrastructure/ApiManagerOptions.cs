namespace TL.Shipay.Project.Infrastructure
{
    public class ApiManagerUrlOptions
    {
        public BrasilApi BrasilApi { get; set; }
        public ViaCep ViaCep { get; set; }
    }

    public class BrasilApi
    {
        public string BaseUrl { get; set; }
        public string DadosCnpj { get; set; }
        public string DadosCep { get; set; }
    }

    public class ViaCep
    {
        public string BaseUrl { get; set; }
        public string DadosCep { get; set; }
    }
}