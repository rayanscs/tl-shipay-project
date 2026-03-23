using TL.Shipay.Project.Domain.ExtensionsMelthods;

namespace TL.Shipay.Project.Domain.Models
{
    public class Endereco
    {
        private string _cep = string.Empty;
        private string _cepFormatado = string.Empty;

        public string Cep
        {
            get => _cep;
            set
            {
                _cep = value ?? string.Empty;
                CepFormatado = StringExtensions.FormataCep(_cep);
            }
        }
        public string CepFormatado
        {
            get => _cepFormatado;
            private set => _cepFormatado = value;
        }
        public string? Cidade { get; set; }
        public string? Logradouro { get; set; }
    }
}
