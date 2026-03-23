using System.ComponentModel;

namespace TL.Shipay.Project.Domain.Utils
{
    public static class EnumExtensions
    {

        /// <summary>
        /// Recupera a descrição do enumerador definida no atributo DescriptionAttribute
        /// </summary>
        /// <param name="pEnumerador">Enumerador de origem</param>
        /// <returns>Retorna a DESCRIÇÃO do enumerador</returns>
        public static string GetDescription(this Enum pEnumerador)
        {
            try
            {
                if (pEnumerador == null)
                    return null;

                var field = pEnumerador.GetType().GetField(pEnumerador.ToString());

                if (field == null)
                    return null;

                var descricaoValue = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

                if (descricaoValue == null)
                    return pEnumerador.ToString();

                return descricaoValue.Description;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao recuperar a Descrição do enumerador, verifique se ele está decorado com o Attr [DescriptionAttribute]", ex);
            }
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }
    }
}
