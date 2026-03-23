using Newtonsoft.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using TL.Shipay.Project.Domain.Enums;

namespace TL.Shipay.Project.Domain.ExtensionsMethods
{
    public static class ObjectExtensions
    {
        public static bool PossuiCampo(this object obj, string campo)
        {
            var type = obj.GetType();
            var prop = type.GetProperty(campo, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null) return true;

            foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
                var jsonName = attr?.Name ?? p.Name;
                
                if (string.Equals(jsonName, campo, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static EResilienciaServico IdentificarTypeofEndereco(this object objeto, string campoA, string campoB)
        {
            if (objeto is null) throw new ArgumentNullException(nameof(objeto));

            var possuiCampoBrasilApi = objeto.PossuiCampo(campoA);
            var possuiCampoViaCep = objeto.PossuiCampo(campoB);
            var json = JsonConvert.SerializeObject(objeto);

            return (possuiCampoBrasilApi, possuiCampoViaCep) switch
            {
                (true, false) => EResilienciaServico.BrasilApi,
                (false, true) => EResilienciaServico.ViaCep,
                (true, true) =>  throw new InvalidOperationException("Falha ao desserializar para TA ou TB."),
                _ => throw new ArgumentException("O objeto não pode ser convertido.")
            };
        }
    }
}
