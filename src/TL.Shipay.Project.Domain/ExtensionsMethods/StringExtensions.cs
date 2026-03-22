using System.Text.RegularExpressions;

namespace TL.Shipay.Project.Domain.ExtensionsMelthods
{
    public static class StringExtensions
    {
        public static string CepSomenteNumeros(this string cep) => Regex.Replace(cep ?? string.Empty, @"[\p{L}\.\-]", "");
    }
}
