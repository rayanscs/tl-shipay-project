using Newtonsoft.Json;

namespace TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCep
{
    public class BrasilApiCepResponse
    {
        [JsonProperty("cep")]
        public string? Cep { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("city")]
        public string? City { get; set; }

        [JsonProperty("neighborhood")]
        public string? Neighborhood { get; set; }

        [JsonProperty("street")]
        public string? Street { get; set; }

        [JsonProperty("service")]
        public string? Service { get; set; }

        [JsonProperty("location")]
        public Location? Location { get; set; }
    }
}
