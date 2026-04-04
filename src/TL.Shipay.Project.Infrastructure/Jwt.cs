namespace TL.Shipay.Project.Infrastructure
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public uint ExpiresInMinutes { get; set; }
    }
}
