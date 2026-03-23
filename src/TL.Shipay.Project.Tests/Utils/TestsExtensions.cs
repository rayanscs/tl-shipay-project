namespace TL.Shipay.Project.Tests.Utils
{
    public static class TestsExtensions
    {
        private static readonly Random _random = new Random();

        public static string GerarCnpj()
        {
            int soma = 0, resto = 0;
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            Random rnd = new Random();
            string semente = rnd.Next(10000000, 99999999).ToString() + "0001";

            for (int i = 0; i < 12; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(semente[i].ToString()) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            semente = semente + resto;
            return semente;
        }

        public static string GerarCepFormatado()
        {
            int parte1 = _random.Next(10000, 99999);
            int parte2 = _random.Next(0, 999);
            return $"{parte1:D5}-{parte2:D3}";
        }

        public static string GerarCepSimples()
        {
            return _random.Next(10000000, 99999999).ToString();
        }
    }
}
