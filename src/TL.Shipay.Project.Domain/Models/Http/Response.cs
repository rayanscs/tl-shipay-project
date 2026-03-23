using Newtonsoft.Json;
using TL.Shipay.Project.Domain.Interfaces;

namespace TL.Shipay.Project.Domain.Models.Http
{
    public class Response : IResponse
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

        public IResponse AddNotification(string menssage)
        {
            _notificacoes.Add(new Notification("", "", menssage));
            return this;
        }

        public IResponse AddNotification(string codigo, string titulo, string menssage)
        {
            _notificacoes.Add(new Notification(codigo, titulo, menssage));
            return this;
        }

        public IResponse AddNotification(Notification notification)
        {
            _notificacoes.Add(notification);
            return this;
        }

        public IResponse AddNotifications(IReadOnlyCollection<Notification> notifications)
        {
            _notificacoes.AddRange(notifications);
            return this;
        }

        public IResponse AddNotifications(IList<Notification> notifications)
        {
            _notificacoes.AddRange(notifications);
            return this;
        }

        public IResponse AddNotifications(ICollection<Notification> notifications)
        {
            _notificacoes.AddRange(notifications);
            return this;
        }

        public void SetMensagemPrincipal(string mensagemPrincipal)
        {
            MensagemPrincipal = mensagemPrincipal;
        }

        public IResponse SetData(object obj)
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
