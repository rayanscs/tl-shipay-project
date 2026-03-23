namespace TL.Shipay.Project.Domain.Models.Request;
public class ClienteRequest
{
    public ClienteRequest(string Cnpj, string Cep)
    {
        this.Cnpj = Cnpj;
        this.Cep = Cep;
    }

    public string Cnpj { get; set; }
    public string Cep { get; set; }
}