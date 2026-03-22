using TL.Shipay.Project.Domain.Models.Http;

namespace TL.Shipay.Project.Domain.Interfaces
{
    public interface IResponse
    {
        object Data { get; }
        bool Sucesso { get; }
        IReadOnlyCollection<Notification> Notifications { get; }
        string MensagemPrincipal { get; }

        IResponse AddNotification(string menssage);
        IResponse AddNotification(string codigo, string titulo, string menssage);
        IResponse AddNotification(Notification notification);
        IResponse AddNotifications(IList<Notification> notifications);

        void SetMensagemPrincipal(string mensagemPrincipal);
        IResponse SetData(object obj);
        T GetDataJson<T>();
        T GetData<T>();
    }
}
