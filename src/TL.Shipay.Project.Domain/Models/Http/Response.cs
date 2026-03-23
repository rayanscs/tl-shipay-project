using Newtonsoft.Json;

namespace TL.Shipay.Project.Domain.Models.Http
{
    public class Response
    {
        public object Data { get; private set; }

        public string MensagemPrincipal { get; set; }

        private readonly List<Notification> _notificacoes;

        public bool Sucesso => (!_notificacoes.Any());

        public Response()
        {
            _notificacoes = new List<Notification>();
        }

        public IReadOnlyCollection<Notification> Notifications => _notificacoes;

        public Response AddNotification(string menssage)
        {
            _notificacoes.Add(new Notification("", "", menssage));
            return this;
        }

        public Response AddNotification(string codigo, string titulo, string menssage)
        {
            _notificacoes.Add(new Notification(codigo, titulo, menssage));
            return this;
        }

        public Response AddNotification(Notification notification)
        {
            _notificacoes.Add(notification);
            return this;
        }

        public Response AddNotifications(IReadOnlyCollection<Notification> notifications)
        {
            _notificacoes.AddRange(notifications);
            return this;
        }

        public Response AddNotifications(IList<Notification> notifications)
        {
            _notificacoes.AddRange(notifications);
            return this;
        }

        public Response AddNotifications(ICollection<Notification> notifications)
        {
            _notificacoes.AddRange(notifications);
            return this;
        }

        public void SetMensagemPrincipal(string mensagemPrincipal)
        {
            MensagemPrincipal = mensagemPrincipal;
        }

        public Response SetData(object obj)
        {
            Data = obj;
            return this;
        }

        public T GetDataJson<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Data.ToString());
            }
            catch (Exception ex)
            {
                return default;
            }
        }
        public T GetData<T>() => (T)Data;
    }
}
