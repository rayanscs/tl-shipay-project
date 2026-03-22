namespace TL.Shipay.Project.Domain.Models.Http
{
    public sealed class Notification
    {
        public Notification(string codigo, string titulo, string mensagem)
        {
            Codigo = codigo;
            Titulo = titulo;
            Mensagem = mensagem;
        }

        public Notification(string mensagem)
        {
            Mensagem = mensagem;
        }

        public string Codigo { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
    }
}
