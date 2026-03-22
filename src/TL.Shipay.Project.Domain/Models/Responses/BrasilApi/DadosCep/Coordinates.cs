using Newtonsoft.Json;

namespace TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCep
{
    public class Coordinates
    {
        [JsonProperty("longitude")]
        public string? Longitude { get; set; }

        [JsonProperty("latitude")]
        public string? Latitude { get; set; }
    }
}
