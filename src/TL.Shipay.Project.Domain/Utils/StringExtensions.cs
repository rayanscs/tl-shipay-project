using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TL.Shipay.Project.Domain.Utils
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

        public static string NormalizaString(string? s, bool ehlogradouro = true)
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

            var normalized = sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();

            if (ehlogradouro) normalized = RemoverPalavrasReservadasEndereco(normalized);

            return normalized.Trim();
        }

        private static string RemoverPalavrasReservadasEndereco(string s)
        {
            // Lista de palavras-chave a remover (com vírgulas opcionais)
            var keywords = new[] { "RUA", "R.", "AVENIDA", "AV." };

            foreach (var keyword in keywords)
            {
                // Remove a palavra-chave seguida por vírgula e espaços
                s = Regex.Replace(s, @$"\b{Regex.Escape(keyword)}\s*,?\s*", string.Empty, RegexOptions.IgnoreCase);
            }

            return s.Trim();
        }
    }
}
