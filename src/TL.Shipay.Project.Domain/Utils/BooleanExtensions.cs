namespace TL.Shipay.Project.Domain.Utils
{
    public static class BooleanExtensions
    {
        public static int ToInt(this bool value) => value ? 1 : 0;

        public static bool ToBoolean(this uint value) => value > 0;

    }
}
