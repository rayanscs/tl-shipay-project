using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TL.Shipay.Project.Domain.ExtensionsMelthods
{
    public static class StringExtensions
    {
        public static string CepSomenteNumeros(this string cep) => Regex.Replace(cep ?? string.Empty, @"[\p{L}\.\-]", "");

        public static string FormataCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
                return string.Empty;

            var digitos = Regex.Replace(cep, @"\D", "");

            if (digitos.Length > 8)
                return string.Empty;

            return digitos.Substring(0, 5) + "-" + digitos.Substring(5, 3);
        }

        public static string NormalizaString(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var trimmed = s.Trim();
            var formD = trimmed.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
        }
    }
}
