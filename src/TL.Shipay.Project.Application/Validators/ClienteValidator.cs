using FluentValidation;
using System.Text.RegularExpressions;
using TL.Shipay.Project.Domain.Models.Request;

namespace TL.Shipay.Project.Application.Validators
{
    public sealed class ClienteValidator : AbstractValidator<ClienteRequest>
    {
        public ClienteValidator()
        {
            RuleFor(x => x.Cnpj)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("O CNPJ é obrigatório.")
                .Must(cnpj =>
                {
                    if (!Regex.IsMatch(cnpj, @"\p{L}"))
                        return true;

                    var agora = DateTime.Now;
                    var limite = new DateTime(2026, 7, 1);
                    return agora >= limite;
                })
                .WithMessage("Cnpj alfanumérico só pode ser utilizado a partir de jul-2026. Favor utilizar o padrão somente números.")
                .MaximumLength(14).WithMessage("O CNPJ deve ter no máximo 14 caracteres.")
                .Must(ValidarCnpj).WithMessage("O CNPJ informado é inválido.");

            RuleFor(x => x.Cep)
                .NotEmpty().WithMessage("O CEP é obrigatório.")
                .Matches(@"^\d{5}-?\d{3}$").WithMessage("O formato do CEP é inválido.");
        }

        private bool ValidarCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

            if (cnpj.Length != 14)
                return false;

            if (cnpj == "00000000000000" || string.Join("", cnpj.Distinct()) == cnpj[0].ToString())
                return false;

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Calcula o primeiro dígito verificador
            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;
            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;

            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;
            digito = digito + resto.ToString();

            // Verifica se os dígitos calculados batem com os digitados
            return cnpj.EndsWith(digito);
        }
    }
}
