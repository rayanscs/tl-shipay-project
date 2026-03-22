using Newtonsoft.Json;

namespace TL.Shipay.Project.Domain.Models.Responses.BrasilApi.DadosCep
{
    public class Location
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("coordinates")]
        public Coordinates? Coordinates { get; set; }
    }
}
